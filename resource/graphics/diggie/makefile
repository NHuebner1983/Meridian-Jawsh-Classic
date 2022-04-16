TOPDIR=..\..\..
!include $(TOPDIR)\common.mak

SOURCEDIR = .
OUTDIR = .

.SUFFIXES: .bbg

BGFS = neeleme.bgf neelemeX.bgf iunholyt.bgf xeoneru.bgf xeoneruX.bgf \
       pear.bgf peartree.bgf orange.bgf orangetree.bgf cookie.bgf \
       mmdrat.bgf mfdrat.bgf arrowgold.bgf brc.bgf brd.bgf brj.bgf \
       brk.bgf brl.bgf brm.bgf mugo.bgf povbeermug.bgf  tikitrib.bgf \
       earthdrag.bgf earthdragX.bgf weed.bgf weed2.bgf \
       weed3.bgf weed4.bgf weed5.bgf weed6.bgf weed7.bgf weedplant.bgf \
       blckdrag.bgf blckdragX.bgf povwand.bgf wand.bgf wandov.bgf \
       squidS.bgf squidT.bgf wolfmask.bgf frontman.bgf ricehat.bgf squidC.bgf \
       Knight.bgf KnightX.bgf Cknight.bgf CknightX.bgf Hknight.bgf HknightX.bgf \
       corncob.bgf cornbasket.bgf mashtaters.bgf stuffing.bgf turkey.bgf ham.bgf \
       hamslices.bgf rouladen.bgf pumpslice.bgf pumppie.bgf pecanslice.bgf \
       pecanpie.bgf indianhat.bgf indianhatF.bgf pilgrimhat.bgf pilgrimhatF.bgf \
       ogre.bgf ogreX.bgf wizardhat.bgf wizardhatF.bgf headless.bgf key1.bgf \
       key2.bgf key3.bgf key4.bgf key5.bgf key6.bgf key7.bgf souls.bgf vajra.bgf \
       vajraX.bgf chimera.bgf chimeraX.bgf cyclops.bgf cyclopsX.bgf followerofqor.bgf \
       followerofqorX.bgf minotaurb.bgf minotaurbX.bgf wasp.bgf waspX.bgf tyrant.bgf \
       tyrantX.bgf fallenmonk.bgf fallenmonkX.bgf vozun.bgf vozunX.bgf croco.bgf \
       crocoX.bgf dulahan.bgf dulahanX.bgf grandpatrent.bgf grandpatrentX.bgf \
       spitter.bgf spitterX.bgf crusher.bgf crusherX.bgf frenzy.bgf frenzyX.bgf \
       hydrab.bgf hydrabX.bgf hydrag.bgf hydragX.bgf hydrar.bgf hydrarX.bgf hydray.bgf \
       hydrayX.bgf greyseeker.bgf greenseeker.bgf brownseeker.bgf dthmitea.bgf dthmiteaX.bgf \
       dthmiteb.bgf dthmitebX.bgf dthmitec.bgf dthmitecX.bgf dthmited.bgf dthmitedX.bgf \
       reavera.bgf reaveraX.bgf reaverb.bgf reaverbX.bgf reaverc.bgf reavercX.bgf 


!include $(RESOURCEDIR)\copy.mak

{$(SOURCEDIR)}.bbg.bgf:
   $(MAKEBGF) -o $(OUTDIR)\$*.bgf @$*.bbg

clean:
