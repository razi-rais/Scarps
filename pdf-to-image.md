## Installation

```
brew install freetype imagemagick
```

```
#########################################################
## pdf-to-image.py
## Covert pdf file into image with desired resolution
#########################################################

from wand.image import Image
# Ref: https://stackoverflow.com/questions/46184239/extract-a-page-from-a-pdf-as-a-jpeg

f = "somefile.pdf"
with(Image(filename=f, resolution=120)) as source: 
    for i, image in enumerate(source.sequence):
        newfilename = f[:-4] + str(i + 1) + '.jpeg'
        Image(image).save(filename=newfilename)
```
