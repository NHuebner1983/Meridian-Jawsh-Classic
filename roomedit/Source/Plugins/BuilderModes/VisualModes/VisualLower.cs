
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal sealed class VisualLower : BaseVisualGeometrySidedef
	{
		#region ================== Constants

		#endregion

		#region ================== Variables
		
		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Setup

		// Constructor
		public VisualLower(BaseVisualMode mode, VisualSector vs, Sidedef s) : base(mode, vs, s)
		{
			//mxd
			geometrytype = VisualGeometryType.WALL_LOWER;
			partname = "bottom";
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// This builds the geometry. Returns false when no geometry created.
		private bool SetupMeridian()
		{
			Vector2D vl, vr;
			float h0, h1, h2, h3;
			bool drawTopDown;

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);

			// Load sector data
			SectorData sd = Sector.GetSectorData();
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if (!osd.Updated)
				osd.Update();

			CalculateWallSideHeights();

			// Left and right vertices for this sidedef
			if(Sidedef.IsFront)
			{
				vl = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				vr = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				if (BowtieFlags.IsBelowPos || BowtieFlags.IsBelowNeg)
				{
					h0 = z1Neg;
					h3 = zz1Neg;
				}
				else
				{
					h0 = z1;
					h3 = zz1;
				}

				h1 = z0;
				h2 = zz0;
				drawTopDown = Sidedef.Line.IsFlagSet("16384");
			}
			else
			{
				vl = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				vr = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				h0 = zz1;
				h1 = zz0;
				h2 = z0;
				h3 = z1;
				drawTopDown = Sidedef.Line.IsFlagSet("32768");
			}

			// Texture given?
			if(Sidedef.LongLowTexture != MapSet.EmptyLongName)
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongLowTexture);
				if(base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = Sidedef.LongLowTexture;
				}
				else
				{
					if(!base.Texture.IsImageLoaded)
						setuponloadedtexture = Sidedef.LongLowTexture;
				}
			}
			else
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}

			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			float floorbias = (osd.Floor.plane.GetZ(vr.x, vr.y) <= sd.Floor.plane.GetZ(vr.x, vr.y)) ? 1.0f : 0.0f;
	
			TexturePlane tp = CalculateTexturePlane(h0, h1, h2, h3, drawTopDown);

			tp.vlt = new Vector3D(vl.x, vl.y, h0);
			tp.vrt = new Vector3D(vr.x, vr.y, h3);
			tp.vrb = new Vector3D(vr.x, vr.y, h2);
			tp.vlb = new Vector3D(vl.x, vl.y, h1);

			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, osd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vr.x, vr.y, osd.Floor.plane.GetZ(vr)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));
			
			// Determine initial color
			int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;

			//mxd. This calculates light with doom-style wall shading
			PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
			PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
			fogfactor = CalculateFogFactor(lightlevel);
			poly.color = wallcolor.WithAlpha(255).ToInt();
			
			// Cut off the part above the other floor
			CropPoly(ref poly, osd.Floor.plane, false);
			CropPoly(ref poly, osd.Ceiling.plane, true);

			// Cut out pieces that overlap 3D floors in this sector
			List<WallPolygon> polygons = new List<WallPolygon> { poly };
			ClipExtraFloors(polygons, sd.ExtraFloors, false); //mxd

			if(polygons.Count > 0)
			{
				// Keep top and bottom planes for intersection testing
				Vector2D linecenter = Sidedef.Line.GetCenterPoint(); //mxd. Our sector's ceiling can be lower than the other sector's floor!
				top = (osd.Floor.plane.GetZ(linecenter) < sd.Ceiling.plane.GetZ(linecenter) ? osd.Floor.plane : sd.Ceiling.plane);
				bottom = sd.Floor.plane;
				
				// Process the polygon and create vertices
				List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
				if(verts.Count > 2)
				{
					base.SetVertices(verts);
					return true;
				}
			}
			
			base.SetVertices(null); //mxd
			return false;
		}

		// This builds the geometry. Returns false when no geometry created.
		public override bool Setup()
		{
			if (General.Map.MERIDIAN)
				return SetupMeridian();

			Vector2D vl, vr;

			//mxd. lightfog flag support
			int lightvalue;
			bool lightabsolute;
			GetLightValue(out lightvalue, out lightabsolute);
			
			Vector2D tscale = new Vector2D(Sidedef.Fields.GetValue("scalex_bottom", 1.0f),
										   Sidedef.Fields.GetValue("scaley_bottom", 1.0f));
			Vector2D toffset = new Vector2D(Sidedef.Fields.GetValue("offsetx_bottom", 0.0f),
											Sidedef.Fields.GetValue("offsety_bottom", 0.0f));
			
			// Left and right vertices for this sidedef
			if(Sidedef.IsFront)
			{
				vl = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
				vr = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
			}
			else
			{
				vl = new Vector2D(Sidedef.Line.End.Position.x, Sidedef.Line.End.Position.y);
				vr = new Vector2D(Sidedef.Line.Start.Position.x, Sidedef.Line.Start.Position.y);
			}
			
			// Load sector data
			SectorData sd = Sector.GetSectorData();
			SectorData osd = mode.GetSectorData(Sidedef.Other.Sector);
			if(!osd.Updated) osd.Update();
			
			// Texture given?
			if(Sidedef.LongLowTexture != MapSet.EmptyLongName)
			{
				// Load texture
				base.Texture = General.Map.Data.GetTextureImage(Sidedef.LongLowTexture);
				if(base.Texture == null || base.Texture is UnknownImage)
				{
					base.Texture = General.Map.Data.UnknownTexture3D;
					setuponloadedtexture = Sidedef.LongLowTexture;
				}
				else
				{
					if(!base.Texture.IsImageLoaded)
						setuponloadedtexture = Sidedef.LongLowTexture;
				}
			}
			else
			{
				// Use missing texture
				base.Texture = General.Map.Data.MissingTexture3D;
				setuponloadedtexture = 0;
			}
			
			// Get texture scaled size
			Vector2D tsz = new Vector2D(base.Texture.ScaledWidth, base.Texture.ScaledHeight);
			tsz = tsz / tscale;
			
			// Get texture offsets
			Vector2D tof = new Vector2D(Sidedef.OffsetX, Sidedef.OffsetY);
			tof = tof + toffset;
			tof = tof / tscale;
			if(General.Map.Config.ScaledTextureOffsets && !base.Texture.WorldPanning)
				tof = tof * base.Texture.Scale;
			
			// Determine texture coordinates plane as they would be in normal circumstances.
			// We can then use this plane to find any texture coordinate we need.
			// The logic here is the same as in the original VisualMiddleSingle (except that
			// the values are stored in a TexturePlane)
			// NOTE: I use a small bias for the floor height, because if the difference in
			// height is 0 then the TexturePlane doesn't work!
			TexturePlane tp = new TexturePlane();
			float floorbias = (Sidedef.Other.Sector.FloorHeight == Sidedef.Sector.FloorHeight) ? 1.0f : 0.0f;
			if(Sidedef.Line.IsFlagSet(General.Map.Config.LowerUnpeggedFlag))
			{
				if(Sidedef.Sector.CeilTexture == General.Map.Config.SkyFlatName && Sidedef.Other.Sector.CeilTexture == General.Map.Config.SkyFlatName) 
				{
					// mxd. Replicate Doom texture offset glitch when front and back sector's ceilings are sky
					tp.tlt.y = (float)Sidedef.Other.Sector.CeilHeight - Sidedef.Other.Sector.FloorHeight;
				} 
				else 
				{
					// When lower unpegged is set, the lower texture is bound to the bottom
					tp.tlt.y = (float) Sidedef.Sector.CeilHeight - Sidedef.Other.Sector.FloorHeight;
				}
			}
			tp.trb.x = tp.tlt.x + (float)Math.Round(Sidedef.Line.Length); //mxd. (G)ZDoom snaps texture coordinates to integral linedef length
			tp.trb.y = tp.tlt.y + (Sidedef.Other.Sector.FloorHeight - (Sidedef.Sector.FloorHeight + floorbias));
			
			// Apply texture offset
			tp.tlt += tof;
			tp.trb += tof;
			
			// Transform pixel coordinates to texture coordinates
			tp.tlt /= tsz;
			tp.trb /= tsz;
			
			// Left top and right bottom of the geometry that
			tp.vlt = new Vector3D(vl.x, vl.y, Sidedef.Other.Sector.FloorHeight);
			tp.vrb = new Vector3D(vr.x, vr.y, Sidedef.Sector.FloorHeight + floorbias);
			
			// Make the right-top coordinates
			tp.trt = new Vector2D(tp.trb.x, tp.tlt.y);
			tp.vrt = new Vector3D(tp.vrb.x, tp.vrb.y, tp.vlt.z);
			
			// Create initial polygon, which is just a quad between floor and ceiling
			WallPolygon poly = new WallPolygon();
			poly.Add(new Vector3D(vl.x, vl.y, sd.Floor.plane.GetZ(vl)));
			poly.Add(new Vector3D(vl.x, vl.y, sd.Ceiling.plane.GetZ(vl)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Ceiling.plane.GetZ(vr)));
			poly.Add(new Vector3D(vr.x, vr.y, sd.Floor.plane.GetZ(vr)));
			
			// Determine initial color
			int lightlevel = lightabsolute ? lightvalue : sd.Ceiling.brightnessbelow + lightvalue;

			//mxd. This calculates light with doom-style wall shading
			PixelColor wallbrightness = PixelColor.FromInt(mode.CalculateBrightness(lightlevel, Sidedef));
			PixelColor wallcolor = PixelColor.Modulate(sd.Ceiling.colorbelow, wallbrightness);
			fogfactor = CalculateFogFactor(lightlevel);
			poly.color = wallcolor.WithAlpha(255).ToInt();
			
			// Cut off the part above the other floor
			CropPoly(ref poly, osd.Floor.plane, false);
			CropPoly(ref poly, osd.Ceiling.plane, true);

			// Cut out pieces that overlap 3D floors in this sector
			List<WallPolygon> polygons = new List<WallPolygon> { poly };
			ClipExtraFloors(polygons, sd.ExtraFloors, false); //mxd

			if(polygons.Count > 0)
			{
				// Keep top and bottom planes for intersection testing
				Vector2D linecenter = Sidedef.Line.GetCenterPoint(); //mxd. Our sector's ceiling can be lower than the other sector's floor!
				top = (osd.Floor.plane.GetZ(linecenter) < sd.Ceiling.plane.GetZ(linecenter) ? osd.Floor.plane : sd.Ceiling.plane);
				bottom = sd.Floor.plane;
				
				// Process the polygon and create vertices
				List<WorldVertex> verts = CreatePolygonVertices(polygons, tp, sd, lightvalue, lightabsolute);
				if(verts.Count > 2)
				{
					base.SetVertices(verts);
					return true;
				}
			}
			
			base.SetVertices(null); //mxd
			return false;
		}
		
		#endregion

		#region ================== Methods

		// Return texture name
		public override string GetTextureName()
		{
			return this.Sidedef.LowTexture;
		}
		
		// This changes the texture
		protected override void SetTexture(string texturename)
		{
			this.Sidedef.SetTextureLow(texturename);
			General.Map.Data.UpdateUsedTextures();
			this.Setup();

			//mxd. Other sector also may require updating
			SectorData sd = mode.GetSectorData(Sidedef.Sector);
			if(sd.ExtraFloors.Count > 0)
				((BaseVisualSector)mode.GetVisualSector(Sidedef.Sector)).Rebuild();
		}

		protected override void SetTextureOffsetX(int x)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, (float)x);
		}

		protected override void SetTextureOffsetY(int y)
		{
			Sidedef.Fields.BeforeFieldsChange();
			Sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, (float)y);
		}

		protected override void MoveTextureOffset(int offsetx, int offsety)
		{
			Sidedef.Fields.BeforeFieldsChange();
			float oldx = Sidedef.Fields.GetValue("offsetx_bottom", 0.0f);
			float oldy = Sidedef.Fields.GetValue("offsety_bottom", 0.0f);
			float scalex = Sidedef.Fields.GetValue("scalex_bottom", 1.0f);
			float scaley = Sidedef.Fields.GetValue("scaley_bottom", 1.0f);
			bool textureloaded = (Texture != null && Texture.IsImageLoaded); //mxd
			Sidedef.Fields["offsetx_bottom"] = new UniValue(UniversalType.Float, GetRoundedTextureOffset(oldx, offsetx, scalex, textureloaded ? Texture.Width : -1)); //mxd
			Sidedef.Fields["offsety_bottom"] = new UniValue(UniversalType.Float, GetRoundedTextureOffset(oldy, offsety, scaley, textureloaded ? Texture.Height : -1)); //mxd
		}

		protected override Point GetTextureOffset()
		{
			float oldx = Sidedef.Fields.GetValue("offsetx_bottom", 0.0f);
			float oldy = Sidedef.Fields.GetValue("offsety_bottom", 0.0f);
			return new Point((int)oldx, (int)oldy);
		}

		//mxd
		protected override void ResetTextureScale() 
		{
			Sidedef.Fields.BeforeFieldsChange();
			if(Sidedef.Fields.ContainsKey("scalex_bottom")) Sidedef.Fields.Remove("scalex_bottom");
			if(Sidedef.Fields.ContainsKey("scaley_bottom")) Sidedef.Fields.Remove("scaley_bottom");
		}

		//mxd
		public override void OnTextureFit(FitTextureOptions options) 
		{
			if(!General.Map.UDMF) return;
			if(!Sidedef.LowRequired() || string.IsNullOrEmpty(Sidedef.LowTexture) || Sidedef.LowTexture == "-" || !Texture.IsImageLoaded) return;
			FitTexture(options);
			Setup();
		}
		
		#endregion
	}
}