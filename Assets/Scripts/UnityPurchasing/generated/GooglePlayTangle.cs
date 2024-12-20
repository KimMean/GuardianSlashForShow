// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("DNaa/G/yVSWsMI/RTlXRFsvFrID3un4bd+YZeZHbxQn0w9LgevZqHz/XW74gPt3bckvB3n7QoM4Q8I1wY2Lhb41HnarvICyBAZQbTNz0owG1IN8s5NoCikvjy5wTdisIY4hvVoFX3LOJIayBaBEAnI8ao770NiFZ76a5BZJu2cWoFZIskUu/G2gHDeIllxQ3JRgTHD+TXZPiGBQUFBAVFuqZJoLmmeJL9mbFAsl9AzRh5S0nc3//Vue6pXIP+bucI5letyBg7o3jmP4ZIYh9CP4jELx9uFfMH4zL3pcUGhUllxQfF5cUFBXDq/Q5WbrxEhfsyV25TeDb+picbUA4ydZ6lM7NB6wBOdBUVWgLg4rHazrLkGiS5da++5h6at4/XhcWFBUU");
        private static int[] order = new int[] { 12,4,11,7,9,5,8,12,10,12,13,12,13,13,14 };
        private static int key = 21;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
