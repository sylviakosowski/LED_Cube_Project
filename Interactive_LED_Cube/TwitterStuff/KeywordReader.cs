using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Interactive_LED_Cube
{
    /* 
     * Class to read keywords from a text file and organize them in such
     * a way that it will be easy to map keywords to behaviors in the Cube.
     * 
     * The text file that these keywords are taken from should be a file
     * with each keyword on its own line, and no other text.
     */
    public class KeywordReader
    {
        private string keywordsFile; //Name of file containing keywords we want.
        private string[] keywordArray; //Array to store keywords
        private Dictionary<string, int> keywordDict;

        //All keywords combined into single string, delimited by commas.
        private string combinedString;

        /* Constructor, takes in a file of keywords. */
        public KeywordReader(string keywordsFile)
        {
            this.keywordsFile = keywordsFile;
            keywordDict = new Dictionary<string, int>();
        }

        public string[] KeywordArray
        {
            get { return keywordArray; }
        }

        public string CombinedString
        {
            get { return combinedString; }
        }

        public Dictionary<string, int> KeywordDict
        {
            get { return keywordDict; }
        }

        /* 
         * Given a file, populates keywordArray with strings containing all the 
         * keywords in that file, initializes combined string to be a combined
         * string containing all of those keywords, and populates keywordDict
         * with all the keywords in the array, paired up with a number from 0
         * to keywordArray.Length - 1, in the order that the strings appear
         * in keywordArray.
         */
        public void ExtractKeywords()
        {
            keywordArray = System.IO.File.ReadAllLines(keywordsFile);
            combinedString = string.Join(",", keywordArray);
            combinedString = combinedString.Trim();

            int count = 0;
            foreach(string keyword in keywordArray)
            {
                keywordDict.Add(keyword, count);
                count++;
            }
        }

    }
}
