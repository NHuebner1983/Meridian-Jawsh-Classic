TOPDIR=..\..\..
!include $(TOPDIR)\common.mak

SOURCEDIR = .
OUTDIR = .

.SUFFIXES: .bbg

BGFS = idonate.bgf itaunt.bgf splash.bgf bloodcirclet.bgf bloodcircletov.bgf flaghinv.bgf \
       flagholy.bgf flagkinv.bgf flagkoc.bgf flaguinv.bgf flagunholy.bgf gshshbk.bgf \
       icampfire.bgf platinum.bgf

!include $(RESOURCEDIR)\copy.mak

{$(SOURCEDIR)}.bbg.bgf:
   $(MAKEBGF) -o $(OUTDIR)\$*.bgf @$*.bbg

clean:
