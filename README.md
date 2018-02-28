# UnityStudio
Implement fake sprite .meta generator, import sprite atlas directly to unity.

- SpriteMetaBuilder is the thing.
I initialize it every load file/folder, add sprite and texture pair every time a sprite read.
In File - Export SpriteMetas is the shortcut to call the method. Select a folder then everything is done.
Only outputs atlas, you can also outputs all sprites by commenting the `continue;` in export method.

Fast implement, forgive my ugly codes.
