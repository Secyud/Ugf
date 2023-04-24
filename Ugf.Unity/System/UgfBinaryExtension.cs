using System.IO;
using Secyud.Ugf;

namespace System
{
    public static class UgfBinaryExtension
    {
       
        public static void Write(this BinaryWriter writer, SpriteContainer sprite)
        {
            writer.Write(sprite.AbName);
            writer.Write(sprite.SpriteName);
        }
        public static SpriteContainer ReadSprite(this BinaryReader reader)
        {
            return new SpriteContainer(reader.ReadString(),reader.ReadString());
        }
        
    }
}