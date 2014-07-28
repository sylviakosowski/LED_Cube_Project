using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autodesk_Interactive_Storytelling;
using System.Collections;

namespace UnitTests
{
    [TestClass]
    public class KeywordUtilTests
    {
        string[] keywords = { "autodesk", "autocad", "autodesk maya"};
        string oneKey = "autocad meep";
        string twoKeyDis = "autodesk blah blah autocad";
        string twoKeyConn = "autodesk maya huehuehue";
        string allKeysInString = "autodesk autocad autodesk maya buahaha";
        string onlyAKeyword = "autodesk";
        string noKeywords = "huehuehue no keywords here sucker!!!!!!!1!!!";
        KeywordUtil ku = new KeywordUtil();

        [TestMethod]
        /* Given a string with one keyword, make sure determineKeywordsFromString 
         * returns correctly. */
        public void TestDetermineKeywordsFromString_OneKeyword()
        {
            ArrayList keywordsPresent = 
                    ku.determineKeywordsFromString(keywords, oneKey);
            Assert.AreEqual(keywordsPresent.Count, 1);
            Assert.AreEqual(keywordsPresent.Contains("autocad"), true);
        }

        [TestMethod]
        /* Given a string with two disconnected keywords, make sure 
         * determineKeywordsFromString returns correctly.
         */
        public void TestDetermineKeywordsFromString_TwoKeywordsDisconnected()
        {
            ArrayList keywordsPresent =
                ku.determineKeywordsFromString(keywords, twoKeyDis);
            Assert.AreEqual(keywordsPresent.Count, 2);
            Assert.AreEqual(keywordsPresent.Contains("autocad"), true);
            Assert.AreEqual(keywordsPresent.Contains("autodesk"), true);
        }

        [TestMethod]
        /* Given a string with two connected keywords, make sure 
         * determineKeywordsFromString returns correctly.
         */
        public void TestDetermineKeywordsFromString_TwoKeywordsConnected()
        {
            ArrayList keywordsPresent =
                ku.determineKeywordsFromString(keywords, twoKeyConn);
            Assert.AreEqual(keywordsPresent.Count, 2);
            Assert.AreEqual(keywordsPresent.Contains("autodesk maya"), true);
            Assert.AreEqual(keywordsPresent.Contains("autodesk"), true);
        }

        [TestMethod]
        /* Given a string with all the keywords in it, make sure 
         * determineKeywordsFromString returns correctly.
         */
        public void TestDetermineKeywordsFromString_AllKeywords()
        {
            ArrayList keywordsPresent =
                ku.determineKeywordsFromString(keywords, allKeysInString);
            Assert.AreEqual(keywordsPresent.Count, 3);
            Assert.AreEqual(keywordsPresent.Contains("autodesk maya"), true);
            Assert.AreEqual(keywordsPresent.Contains("autodesk"), true);
            Assert.AreEqual(keywordsPresent.Contains("autocad"), true);
        }

        [TestMethod]
        /* Given a string which is only a single keyword, make sure 
         * determineKeywordsFromString returns correctly.
         */
        public void TestDetermineKeywordsFromString_OnlyAKeyword()
        {
            ArrayList keywordsPresent =
                ku.determineKeywordsFromString(keywords, onlyAKeyword);
            Assert.AreEqual(keywordsPresent.Count, 1);
            Assert.AreEqual(keywordsPresent.Contains("autodesk"), true);
        }

        [TestMethod]
        /* Given a string with no keywords in it, make sure 
         * determineKeywordsFromString returns correctly.
         */
        public void TestDetermineKeywordsFromString_NoKeyword()
        {
            ArrayList keywordsPresent =
                ku.determineKeywordsFromString(keywords, noKeywords);
            Assert.AreEqual(keywordsPresent.Count, 0);
        }

    }
}
