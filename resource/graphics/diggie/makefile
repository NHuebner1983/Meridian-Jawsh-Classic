TOPDIR=..\..\..
!include $(TOPDIR)\common.mak

SOURCEDIR = .
OUTDIR = .

.SUFFIXES: .bbg

BGFS = neeleme.bgf neelemeX.bgf iunholyt.bgf xeoneru.bgf xeoneruX.bgf \
       pear.bgf peartree.bgf orange.bgf orangetree.bgf cookie.bgf \
       mmdrat.bgf mfdrat.bgf arrowgold.bgf weed.bgf weed2.bgf \
       weed3.bgf weed4.bgf weed5.bgf weed6.bgf weed7.bgf weedplant.bgf 

!include $(RESOURCEDIR)\copy.mak

{$(SOURCEDIR)}.bbg.bgf:
   $(MAKEBGF) -o $(OUTDIR)\$*.bgf @$*.bbg

clean:

