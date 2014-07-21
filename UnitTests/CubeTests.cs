using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autodesk_Interactive_Storytelling;

namespace UnitTests
{
    [TestClass]
    public class CubeTests
    {
        private Hypnocube cube = new Hypnocube();

        [TestMethod]
        /* Test that conversion from coordinates to index is working. */
        public void TestIndexFromCoord()
        {
            Assert.AreEqual(cube.IndexFromCoord(0,0,0), 0);
            Assert.AreEqual(cube.IndexFromCoord(7, 7, 7), 1533);
            Assert.AreEqual(cube.IndexFromCoord(3, 6, 7), 1497);
        }

        [TestMethod]
        /* Change the color of the first LED in the array. */
        public void TestChangeColorLEDFirstPos()
        {
            cube.changeColorLED(0,0,0, 0xFF, 0x2A, 0xCC);
            int index = cube.IndexFromCoord(0,0,0);
            Assert.AreEqual(cube.ColorArray[index], 0xFF);
            Assert.AreEqual(cube.ColorArray[index + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index + 2], 0xCC);
        }

        [TestMethod]
        /* Change the color of the last LED in the array. */
        public void TestChangeColorLEDLastPos()
        {
            cube.changeColorLED(7,7,7, 0xFF, 0x2A, 0xCC);
            int index = cube.IndexFromCoord(7,7,7);
            Assert.AreEqual(cube.ColorArray[index], 0xFF);
            Assert.AreEqual(cube.ColorArray[index + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index + 2], 0xCC);
        }

        [TestMethod]
        /* Change color of two LEDs and make sure they don't affect one another. */
        public void TestChangeColorLEDTwoPos()
        {
            cube.changeColorLED(2, 4, 5, 0xFF, 0x2A, 0xCC);
            cube.changeColorLED(2, 4, 6, 0x12, 0xAA, 0x42);

            int index1 = cube.IndexFromCoord(2, 4, 5);
            Assert.AreEqual(cube.ColorArray[index1], 0xFF);
            Assert.AreEqual(cube.ColorArray[index1 + 1], 0x2A);
            Assert.AreEqual(cube.ColorArray[index1 + 2], 0xCC);

            int index2 = cube.IndexFromCoord(2, 4, 6);
            Assert.AreEqual(cube.ColorArray[index2], 0x12);
            Assert.AreEqual(cube.ColorArray[index2 + 1], 0xAA);
            Assert.AreEqual(cube.ColorArray[index2 + 2], 0x42);

        }
    }
}
