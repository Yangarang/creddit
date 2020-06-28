/** 
Written by: Nathan Del Carmen, Ka Wai Chu
Tested by: Nathan Del Carmen, Ka Wai Chu
Debugged by: Nathan Del Carmen, Ka Wai Chu
Purpose: Check for delimter characters and stop words and edit appropriately in search bar.
**/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Creddit
{
    public class search
    {

        public List<string> parser(string text)
        {
            List<string> parselist = new List<string>();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\n' };

            string[] words = text.Split(delimiterChars);


            for (int i = 0; i < words.Count(); ++i)
            {
                parselist.Add(words[i]);
            }
            if (parselist[parselist.Count - 1] == "")
            {
                parselist.RemoveAt(parselist.Count - 1);
            }
            return parselist;
        }

        public List<string> stopwords(List<string> text)
        {

            string[] listofstopwords = { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "aren't", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't", "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further", "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor", "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours	ourselves", "out", "over", "own", "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll", "we're", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };
            for (int i = 0; i < text.Count(); ++i)
            {
                for (int j = 0; j < listofstopwords.Count(); ++j)
                {
                    if (text[i].ToLower() == listofstopwords[j])
                    {

                        text.RemoveAt(i);
                        stopwords(text);
                    }
                }

            }
            return text;

        }
    }

}