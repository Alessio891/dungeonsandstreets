using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public static class MapTags 
{
    static TextAsset raw_data;
    public static List<Type> Data;

    static MapTags()
    {
        Debug.Log("INIT MAP TAGS");
        LoadData();    
    }

    public static Type GetType(string name)
    {
        foreach(Type t in Data)
        {
            if (t.Name == name)
                return t;
        }
        return null;
    }
    public static int IndexOf(string name)
    {
        for(int i = 0; i < Data.Count; i++)
        {
            if (Data[i].Name == name)
            {
                return i;
            }
        }
        return -1;

    }

    public static List<string> GetTypeList()
    {
        List<string> retval = new List<string>();
        foreach (Type t in Data)
            retval.Add(t.Name);
        return retval;
    }

    public static List<string> GetCategoryList(string type)
    {
        List<string> retVal = new List<string>();
        Type t = GetType(type);
        if (t != null)
        {
            foreach(Category c in t.Categories)
            {
                retVal.Add(c.Name);
            }
        }
        return retVal;
    }

    public static void LoadData()
    {
#if UNITY_EDITOR
        raw_data = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Data/maptags.txt") as TextAsset;
#else
        raw_data = Resources.Load<TextAsset>("Data/maptags");
#endif
        Debug.Log("RawData is " + raw_data);
        Dictionary<string, object> deserialized = MiniJSON.Json.Deserialize(raw_data.text) as Dictionary<string, object>;
        Data = new List<Type>();
        foreach(KeyValuePair<string, object> entry in deserialized)
        {
            Dictionary<string, object> categoryDat = (Dictionary<string, object>)entry.Value;
            Type t = new Type();
            t.Name = entry.Key; 
            Debug.Log("Adding Type " + t.Name);
            t.Categories = new List<Category>();
            foreach(KeyValuePair<string, object> cat in categoryDat)
            {
                Category c = new Category();
                c.Name = cat.Key;
                c.Tags = new List<string>();
                string r = "Adding Category " + c.Name + " with tags: [ ";
                List<object> tags = (List<object>)cat.Value;
                foreach(object o in tags)
                {
                    c.Tags.Add(o.ToString());
                    r += o.ToString() + ",";
                }
                r = r.Substring(0, r.Length - 1) + "]";
                Debug.Log(r);
                t.Categories.Add(c);
            }
            Data.Add(t);
        }
    }
    public static string GetStringData()
    {
        string data = "{";
        foreach (Type t in Data)
        {
            data += t.Serialize() + ",";
        }
        data = data.Substring(0, data.Length - 1) + "}";
        return data;
    }
    public static void SaveData()
    {
#if UNITY_EDITOR
        string data = GetStringData();

        File.WriteAllText(AssetDatabase.GetAssetPath(raw_data), data);
        EditorUtility.SetDirty(raw_data);
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
#endif
    }

    public static List<string> PossibleTags = new List<string>()
    {
        "cafe",
        "car_wash",
        "pharmacy",
        "bar",
        "drinking_water",
        "bbq",
        "biergarten",
        "fast_food",
        "food_court",
        "ice_cream",
        "pub",
        "restaurant",
        "college",
        "driving_school",
        "language_school",
        "library",
        "toy_library",
        "music_school",
        "university",
        "bicycle_parking",
        "bicycle_repair_station",
        "bicyle_rental",
        "boat_rental",
        "boat_sharing",
        "bus_station",
        "car_rental",
        "vehicle_inspection",
        "charging_station",
        "ferry_terminal",
        "fuel",
        "grit_bin",
        "motorcycle_parking",
        "parking",
        "parking_entrance",
        "parking_space",
        "taxi",
        "atm",
        "bank",
        "bureau_de_change",
        "dentist",
        "social_facility",
        "veterinary",
        "arts_centre",
        "casino",
        "cinema",
        "community_centre",
        "fountain",
        "gambling",
        "planetarium",
        "public_bookcase",
        "social_centre",
        "studio",
        "theatre",
        "animal_boarding",
        "animal_shelter",
        "baking_oven",
        "bench",
        "childcare",
        "clock",
        "conference_centre",
        "courthouse",
        "dive_centre",
        "embassy",
        "fire_station",
        "give_box",
        "grave_yard",
        "hunting_stand",
        "internet_cafe",
        "kitchen",
        "marketplace",
        "monastery",
        "photo_booth",
        "place_of_worship",
        "police",
        "post_box",
        "post_depot",
        "post_office",
        "public_bath",
        "ranger_station",
        "recycling",
        "sanitary_dump_station",
        "shelter",
        "shower",
        "telephone",
        "toilets",
        "townhall",
        "vending_machine",
        "waste_basket",
        "waste_disposal",
        "waste_transfer_station",
        "watering_place",
        "water_point",
        "alcohol",
        "bakery",
        "beverages",
        "brewing_supplies",
        "butcher",
        "cheese",
        "chocolate",
        "coffee",
        "confectionery",
        "convenience",
        "deli",
        "dairy",
        "farm",
        "frozen_food",
        "greengrocer",
        "health_food",
        "ice_cream",
        "pasta",
        "pastry",
        "seafood",
        "spices",
        "tea",
        "water",
        "department_store",
        "general",
        "kiosk",
        "mall",
        "supermarket",
        "wholesale",
        "bag",
        "boutique",
        "clothes",
        "fabric",
        "jewelry",
        "leather",
        "sewing",
        "shoes",
        "tailor",
        "watches",
        "wool",
        "charity",
        "second_hand",
        "variety_store",
        "beauty",
        "chemist",
        "cosmetics",
        "hairdresser",
        "hairdresser_supply",
        "hearing_aids",
        "herbalist",
        "massage",
        "medical_supply",
        "nutrition_supplements",
        "optician",
        "perfumery",
        "tattoo",
        "agrarian",
        "appliance",
        "bathroom_furnishing",
        "doityourself",
        "electrical",
        "energy",
        "fireplace",
        "florist",
        "garden_centre",
        "garden_furniture",
        "gas",
        "glaziery",
        "hardware",
        "houseware",
        "locksmith",
        "paint",
        "security",
        "trade",
        "antiques",
        "bed",
        "candles",
        "carpet",
        "curtain",
        "doors",
        "flooring",
        "furniture",
        "household_linen",
        "interior_decoration",
        "kitchen",
        "lighting",
        "tiles",
        "window_blind",
        "computer",
        "robot",
        "electronics",
        "hifi",
        "mobile_phone",
        "radiotechnics",
        "vacuum_cleaner",
        "atv",
        "bicycle",
        "boat",
        "car",
        "car_repair",
        "car_parts",
        "caravan",
        "fuel",
        "fishing",
        "free_flying",
        "golf",
        "hunting",
        "jetski",
        "military_surplus",
        "motorcycle",
        "outdoor",
        "scuba_diving",
        "ski",
        "snowmobile",
        "sports",
        "swimming_pool",
        "trailer",
        "tyres",
        "art",
        "collector",
        "craft",
        "frame",
        "games",
        "model",
        "music",
        "musical_instrument",
        "photo",
        "camera",
        "trophy",
        "video",
        "video_games",
        "anime",
        "books",
        "gift",
        "lottery",
        "newsagent",
        "stationery",
        "ticket",
        "bookmarker",
        "cannabis",
        "copyshop",
        "dry_cleaning",
        "e-cigarette",
        "laundry",
        "money_lender",
        "party",
        "pawnbroker",
        "pet",
        "pet_grooming",
        "pest_control",
        "pyrotechnics",
        "religion",
        "storage_rental",
        "tobacco",
        "toys",
        "travel_agency",
        "weapons",
        "outpost"

    };

    [System.Serializable]
    public class Type
    {
        public string Name;
        public List<Category> Categories;

        public string Serialize() {
            string retVal = "\""+Name+"\":{";
            
            foreach(Category c in Categories)
            {
                retVal += "\"" + c.Name + "\":[";
                if (c.Tags.Count > 0)
                {
                    foreach (string t in c.Tags) 
                    {
                        retVal += "\"" + t + "\",";
                    }
                    retVal = retVal.Substring(0, retVal.Length - 1) + "],";
                }
                else
                    retVal += "],";
            }
            retVal = retVal.Substring(0, retVal.Length - 1) + "}";

            return retVal;
        }

        public Category GetCategory(string name)
        {
            foreach(Category c in Categories)
            {
                if (c.Name == name)
                    return c;
            }
            return null;
        }
        
    }
    [System.Serializable]
    public class Category {
        public string Name;
        public List<string> Tags;
    }
}
