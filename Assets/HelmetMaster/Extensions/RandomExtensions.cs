using System.Collections.Generic;
using UnityEngine;

namespace HelmetMaster.Extensions
{
    public class RandomExtensions
    {
        public static string GetRandomString(int lenght)
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string generated_string = "";

            for(int i = 0; i < lenght; i++)
                generated_string += characters[Random.Range(0, lenght)];

            return generated_string;
        }
        
        public static Vector3 GetRandomPositionWithinRectangle(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector3(UnityEngine.Random.Range(xMin, xMax), UnityEngine.Random.Range(yMin, yMax));
        }

        public static Vector3 GetRandomPositionWithinRectangle(Vector3 lowerLeft, Vector3 upperRight)
        {
            return new Vector3(UnityEngine.Random.Range(lowerLeft.x, upperRight.x),
                UnityEngine.Random.Range(lowerLeft.y, upperRight.y));
        }       
        public static Vector3 GetRandomPositionInLine(Vector3 startPos, Vector3 endPos)
        {
            return new Vector3(UnityEngine.Random.Range(startPos.x, endPos.x),
                UnityEngine.Random.Range(startPos.y, endPos.y), Random.Range(startPos.z, startPos.z));
        }

        // Get a random male name and optionally single letter surname
        public static string GetRandomName(bool isOnlyMale, bool isRandomGender = true, bool withSurname = false)
        {
            List<string> maleNameList = new List<string>()
            {
                "Gabe", "Cliff", "Tim", "Ron", "Jon", "John", "Mike", "Seth", "Alex", "Steve", "Chris", "Will", "Bill",
                "James", "Jim",
                "Ahmed", "Omar", "Peter", "Pierre", "George", "Lewis", "Lewie", "Adam", "William", "Ali", "Eddie", "Ed",
                "Dick", "Robert", "Bob", "Rob",
                "Neil", "Tyson", "Carl", "Chris", "Christopher", "Jensen", "Gordon", "Morgan", "Richard", "Wen", "Wei",
                "Luke", "Lucas", "Noah", "Ivan", "Yusuf",
                "Ezio", "Connor", "Milan", "Nathan", "Victor", "Harry", "Ben", "Charles", "Charlie", "Jack", "Leo",
                "Leonardo", "Dylan", "Steven", "Jeff",
                "Alex", "Mark", "Leon", "Oliver", "Danny", "Liam", "Joe", "Tom", "Thomas", "Bruce", "Clark", "Tyler",
                "Jared", "Brad", "Jason"
            };

            List<string> femaleNameList = new List<string>()
            {
                "Emma", "Olivia", "Sophia", "Ava", "Isabella", "Scarlett", "Zoey", "Addison", "Lily", "Lillian",
                "Natalie", "Hannah", "Aria", "Layla", "Brooklyn", "Alexa",
                "Mia", "Abigail", "Emily", "Charlotte", "Harper", "Madison", "Amelia", "Elizabeth", "Sofia", "Evelyn",
                "Avery", "Chloe", "Ella", "Grace", "Victoria", "Aubrey", "Aaliyah", "Claire", "Zoe", "Penelope",
                "Riley", "Leah", "Audrey", "Savannah", "Allison",
                "Samantha", "Nora", "Skylar", "Camila", "Anna", "Paisley", "Ariana", "Ellie", "Violet", "Stella",
                "Sadie", "Mila", "Gabriella", "Lucy",
                "Arianna", "Kennedy", "Sarah", "Madelyn", "Eleanor", "Kaylee", "Caroline", "Hazel", "Hailey", "Genesis",
                "Kylie", "Autumn", "Piper", "Maya",
                "Nevaeh", "Serenity", "Peyton", "Mackenzie", "Bella", "Eva", "Taylor", "Naomi", "Aubree", "Aurora",
                "Melanie", "Lydia", "Brianna", "Ruby",
                "Katherine", "Ashley", "Alexis", "Alice", "Cora", "Julia", "Madeline", "Faith", "Annabelle", "Alyssa",
                "Isabelle", "Vivian", "Gianna", "Quinn", "Clara",
                "Reagan", "Khloe", "Alexandra", "Hadley", "Eliana", "Sophie", "London", "Elena", "Kimberly", "Bailey",
                "Maria", "Luna", "Willow", "Jasmine",
                "Kinsley", "Valentina", "Kayla", "Delilah", "Andrea", "Natalia", "Lauren", "Morgan", "Rylee", "Sydney",
                "Adalynn", "Mary", "Ximena", "Jade", "Liliana",
                "Brielle", "Ivy", "Trinity", "Josephine", "Adalyn", "Jocelyn", "Emery", "Adeline", "Jordyn", "Ariel",
                "Everly", "Lilly", "Paige",
            };
            if (!isRandomGender)
            {
                if (isOnlyMale)
                {
                    if (withSurname)
                    {
                        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                        return maleNameList[UnityEngine.Random.Range(0, maleNameList.Count)] + " " +
                               alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
                    }
                    
                    return maleNameList[UnityEngine.Random.Range(0, maleNameList.Count)];
                }


                if (withSurname)
                {
                    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                    return femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Count)] + " " +
                           alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
                }


                return femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Count)];
            }

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                if (withSurname)
                {
                    string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                    return femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Count)] + " " +
                           alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
                }

                return femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Count)];
            }

            if (withSurname)
            {
                string alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYWZ";
                return maleNameList[UnityEngine.Random.Range(0, femaleNameList.Count)] + " " +
                       alphabet[UnityEngine.Random.Range(0, alphabet.Length)] + ".";
            }

            return maleNameList[UnityEngine.Random.Range(0, maleNameList.Count)];
        }


        public static string GetRandomCityName()
        {
            List<string> cityNameList = new List<string>()
            {
                "Alabama", "New York", "Old York", "Bangkok", "Lisbon", "Vee", "Agen", "Agon", "Ardok", "Arbok",
                "Kobra", "House", "Noun", "Hayar", "Salma", "Chancellor", "Dascomb", "Payn", "Inglo", "Lorr", "Ringu",
                "Brot", "Mount Loom", "Kip", "Chicago", "Madrid", "London", "Gam",
                "Greenvile", "Franklin", "Clinton", "Springfield", "Salem", "Fairview", "Fairfax", "Washington",
                "Madison",
                "Georgetown", "Arlington", "Marion", "Oxford", "Harvard", "Valley", "Ashland", "Burlington",
                "Manchester", "Clayton",
                "Milton", "Auburn", "Dayton", "Lexington", "Milford", "Riverside", "Cleveland", "Dover", "Hudson",
                "Kingston", "Mount Vernon",
                "Newport", "Oakland", "Centerville", "Winchester", "Rotary", "Bailey", "Saint Mary", "Three Waters",
                "Veritas", "Chaos", "Center",
                "Millbury", "Stockland", "Deerstead Hills", "Plaintown", "Fairchester", "Milaire View", "Bradton",
                "Glenfield", "Kirkmore",
                "Fortdell", "Sharonford", "Inglewood", "Englecamp", "Harrisvania", "Bosstead", "Brookopolis",
                "Metropolis", "Colewood", "Willowbury",
                "Hearthdale", "Weelworth", "Donnelsfield", "Greenline", "Greenwich", "Clarkswich", "Bridgeworth",
                "Normont",
                "Lynchbrook", "Ashbridge", "Garfort", "Wolfpain", "Waterstead", "Glenburgh", "Fortcroft", "Kingsbank",
                "Adamstead", "Mistead",
                "Old Crossing", "Crossing", "New Agon", "New Agen", "Old Agon", "New Valley", "Old Valley",
                "New Kingsbank", "Old Kingsbank",
                "New Dover", "Old Dover", "New Burlington", "Shawshank", "Old Shawshank", "New Shawshank",
                "New Bradton", "Old Bradton", "New Metropolis", "Old Clayton", "New Clayton"
            };
            return cityNameList[UnityEngine.Random.Range(0, cityNameList.Count)];
        }

        public static bool TestChance(int chance, int chanceMax = 100)
        {
            return UnityEngine.Random.Range(0, chanceMax) < chance;
        }

        public static Color GetRandomColor()
        {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
                UnityEngine.Random.Range(0f, 1f), 1f);
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDir()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
        }

        // Generate random normalized direction
        public static Vector3 GetRandomDirXZ()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
        
        public static T GetWeightedRandomItem<T>(List<T> items, List<int> weights)
        {
            int totalPriority = 0;
            List<int> weightsOriginal = new List<int>(weights);
            List<int> tempWeights = new List<int>(weights);
            
            for (int i = 0; i < items.Count; i++)
            {
                weightsOriginal[i] += totalPriority; 
                totalPriority += tempWeights[i]; 
            }

            int randomPriority = Random.Range(0, totalPriority);
        
            for (int i = 0; i < items.Count; i++)
            {
                if (weightsOriginal[i] > randomPriority)
                {
                    return items[i];
                }
            }
            return items[0];
        }
    }
}