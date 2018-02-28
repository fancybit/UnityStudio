using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Unity_Studio
{
    public static class SpriteMetaBuilder
    {
        public static Dictionary<AssetPreloadData, List<Sprite>> Sprites = new Dictionary<AssetPreloadData, List<Sprite>>();
        #region public const string MetaTable = @"fileFormatVersion: 2
        public const string MetaTable = @"fileFormatVersion: 2
guid: {guid}
timeCreated: {timeCreated}
licenseType: Pro
TextureImporter:
  fileIDToRecycleName: {}
  externalObjects: {}
  serializedVersion: 4
  mipmaps:
    mipMapMode: 0
    enableMipMap: 0
    sRGBTexture: 1
    linearTexture: 0
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
  isReadable: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: -1
    aniso: -1
    mipBias: -1
    wrapU: 1
    wrapV: 1
    wrapW: -1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: {spriteMode}
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {x: 0.5, y: 0.5}
  spriteBorder: {x: 0, y: 0, z: 0, w: 0}
  spritePixelsToUnits: {spritePixelsToUnits}
  alphaUsage: 1
  alphaIsTransparency: 1
  spriteTessellationDetail: -1
  textureType: 8
  textureShape: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  platformSettings:
  - buildTarget: DefaultTexturePlatform
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
  - buildTarget: Standalone
    maxTextureSize: 2048
    resizeAlgorithm: 0
    textureFormat: -1
    textureCompression: 1
    compressionQuality: 50
    crunchedCompression: 0
    allowsAlphaSplitting: 0
    overridden: 0
    androidETC2FallbackOverride: 0
  spriteSheet:
    serializedVersion: 2
{sprites}
    outline: []
    physicsShape: []
  spritePackingTag: 
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";
        #endregion
        #region public const string SpriteTable = @"    - serializedVersion: 2
        public const string SpriteTable = @"    - serializedVersion: 2
      name: {name}
      rect:
        serializedVersion: 2
        x: {rect_x}
        y: {rect_y}
        width: {rect_width}
        height: {rect_height}
      alignment: 9
      pivot: {x: {pivot_x}, y: {pivot_y}}
      border: {x: {border_x}, y: {border_y}, z: {border_z}, w: {border_w}}
      outline: []
{physicsShape}
      tessellationDetail: 0";
        #endregion
        public const string PhysicsShapeTable = @"      {-} - {x: {x}, y: {y}}";

        public static void Initialize()
        {
            Sprites.Clear();
        }
        public static void AddPPtrSpriteReference(AssetPreloadData Texture, Sprite Sprite)
        {
            if (!Sprites.ContainsKey(Texture))
            {
                Sprites.Add(Texture, new List<Sprite>());
            }
            List<Sprite> SpriteReferences = Sprites[Texture];
            SpriteReferences.Add(Sprite);
        }
        public static void ExportAllSpriteMetas()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog(System.Windows.Forms.Form.ActiveForm) != System.Windows.Forms.DialogResult.OK) return;
            string FolderPath = openFolderDialog.Folder.Replace("/", "\\");
            FolderPath = FolderPath.EndsWith("\\") ? FolderPath : (FolderPath + "\\");

            foreach (AssetPreloadData Texture in Sprites.Keys)
            {
                string SpriteMeta = MetaTable;
                SpriteMeta = SpriteMeta.Replace("{guid}", Guid.NewGuid().ToString());
                SpriteMeta = SpriteMeta.Replace("{timeCreated}", (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString());

                List<Sprite> ReferencedSprites = Sprites[Texture];
                SpriteMeta = SpriteMeta.Replace("{spritePixelsToUnits}", ReferencedSprites[0].m_PixelsToUnits.ToString());

                if (ReferencedSprites.Count == 1)
                {
                    continue;
                    SpriteMeta = SpriteMeta.Replace("{spriteMode}", "1");
                    SpriteMeta = SpriteMeta.Replace("{sprites}", "    sprites: []");
                    UnityStudio.ExportTexture2D(Texture, FolderPath, true);
                    File.WriteAllText(FolderPath + Texture.Text + ".png.meta", SpriteMeta);
                    continue;
                }

                SpriteMeta = SpriteMeta.Replace("{spriteMode}", "2");
                StringBuilder spriteReplacement = new StringBuilder();
                spriteReplacement.AppendLine("    sprites:");

                foreach (Sprite sprite in ReferencedSprites)
                {
                    string Table = SpriteTable;
                    Table = Table.Replace("{name}", sprite.m_Name);
                    Table = Table.Replace("{rect_x}", sprite.m_Rect.X.ToString());
                    Table = Table.Replace("{rect_y}", sprite.m_Rect.Y.ToString());
                    Table = Table.Replace("{rect_width}", sprite.m_Rect.Width.ToString());
                    Table = Table.Replace("{rect_height}", sprite.m_Rect.Height.ToString());
                    Table = Table.Replace("{pivot_x}", sprite.m_Pivot.X.ToString());
                    Table = Table.Replace("{pivot_y}", sprite.m_Pivot.Y.ToString());
                    Table = Table.Replace("{border_x}", sprite.m_Border.x.ToString());
                    Table = Table.Replace("{border_y}", sprite.m_Border.y.ToString());
                    Table = Table.Replace("{border_z}", sprite.m_Border.z.ToString());
                    Table = Table.Replace("{border_w}", sprite.m_Border.w.ToString());

                    if (sprite.m_PhysicsShape == null)
                    {
                        Table = Table.Replace("{physicsShape}", "      physicsShape: []");
                    }
                    else
                    {
                        StringBuilder physicsShapeReplacement = new StringBuilder();
                        physicsShapeReplacement.AppendLine("      physicsShape:");
                        for (int i = 0; i < sprite.m_PhysicsShape.GetLength(0); ++i)
                        {
                            for (int j = 0; j < sprite.m_PhysicsShape.GetLength(1); ++j)
                            {
                                string pointReplacement = PhysicsShapeTable;
                                pointReplacement = pointReplacement.Replace("{-}", j == 0 ? "-" : " ");
                                pointReplacement = pointReplacement.Replace("{x}", sprite.m_PhysicsShape[i][j].X.ToString());
                                pointReplacement = pointReplacement.Replace("{y}", sprite.m_PhysicsShape[i][j].Y.ToString());
                                physicsShapeReplacement.AppendLine(pointReplacement);
                            }
                        }
                        Table = Table.Replace("{physicsShape}", physicsShapeReplacement.ToString());
                    }

                    spriteReplacement.AppendLine(Table);
                }

                SpriteMeta = SpriteMeta.Replace("{sprites}", spriteReplacement.ToString());
                UnityStudio.ExportTexture2D(Texture, FolderPath, true);
                File.WriteAllText(FolderPath + Texture.Text + ".png.meta", SpriteMeta);
            }
        }
    }
}
