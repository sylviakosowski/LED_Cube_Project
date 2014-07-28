using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autodesk_Interactive_Storytelling;

namespace UnitTests
{
    [TestClass]
    public class CubeTests
    {
        private HypnocubeImpl cube = new HypnocubeImpl();

        [TestMethod]
        /* Test that conversion from coordinates to index is working. */
        public void TestIndexFromCoord()
        {
            Assert.AreEqual(cube.IndexFromCoord( new Coordinate(0,0,0)), 0);
            Assert.AreEqual(cube.IndexFromCoord( new Coordinate(7, 7, 7)), 1533);
            Assert.AreEqual(cube.IndexFromCoord( new Coordinate(3, 6, 7)), 1497);
        }

        [TestMethod]
        /* Change the color of the first LED in the array. */
        public void TestChangeColorLEDFirstPos()
        {
            Coordinate zero = new Coordinate(0, 0, 0);
            cube.changeColorLED(zero, new RGBColor(0xFF, 0x2A, 0xCC), false);
            int index = cube.IndexFromCoord(zero);
            Assert.AreEqual(cube.ColorArray[index], 0xFF);
            Assert.AreEqual(cube.ColorArray[index + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index + 2], 0xCC);
        }

        [TestMethod]
        /* Change the color of the last LED in the array. */
        public void TestChangeColorLEDLastPos()
        {
            Coordinate c = new Coordinate(7,7,7);
            cube.changeColorLED(c, new RGBColor(0xFF, 0x2A, 0xCC), false);
            int index = cube.IndexFromCoord(c);
            Assert.AreEqual(cube.ColorArray[index], 0xFF);
            Assert.AreEqual(cube.ColorArray[index + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index + 2], 0xCC);
        }

        [TestMethod]
        /* Change color of two LEDs and make sure they don't affect one another. */
        public void TestChangeColorLEDTwoPos()
        {
            Coordinate c1 = new Coordinate(2, 4, 5);
            Coordinate c2 = new Coordinate(2, 4, 6);

            cube.changeColorLED(c1, new RGBColor(0xFF, 0x2A, 0xCC), false);
            cube.changeColorLED(c2, new RGBColor(0x12, 0xAA, 0x42), false);

            int index1 = cube.IndexFromCoord(c1);
            Assert.AreEqual(cube.ColorArray[index1], 0xFF);
            Assert.AreEqual(cube.ColorArray[index1 + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index1 + 2], 0xCC);

            int index2 = cube.IndexFromCoord(c2);
            Assert.AreEqual(cube.ColorArray[index2], 0x12);
            Assert.AreEqual(cube.ColorArray[index2 + 1], 0xAA);
            Assert.AreEqual(cube.ColorArray[index2 + 2], 0x42);

        }
    }
}
