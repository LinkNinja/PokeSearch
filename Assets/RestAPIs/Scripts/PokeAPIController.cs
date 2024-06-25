using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class PokeAPIController : MonoBehaviour
{
    public RawImage pokeRawImage;
    public TextMeshProUGUI pokeNameText, pokeNumText;
    public TextMeshProUGUI[] pokeTypeTextArray;
    public TextMeshProUGUI[] pokeStatsNameTextArray;
    public TextMeshProUGUI[] pokeBaseValuesTextArray;
    public TMP_InputField pokemonSearchName;


    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    private void Start()
    {
        pokeRawImage.texture = Texture2D.blackTexture;
        pokeNameText.text = "";
        pokeNumText.text = "";
        
        //Blank out the pokemon types at the start of the application
        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }

        foreach (TextMeshProUGUI pokeBaseValuesText in pokeBaseValuesTextArray)
        {
            pokeBaseValuesText.text = "";
        }
       

    }

    public void OnButtonSearchPokemon()
    {



        //when the button is pressed the white texture is defaulted to black.
        pokeRawImage.texture = Texture2D.blackTexture;

        
        pokeNameText.text = "Loading...";

        pokeNumText.text = "#" + pokeNumText.text;

        foreach (TextMeshProUGUI pokeTypeText in pokeTypeTextArray)
        {
            pokeTypeText.text = "";
        }
        foreach (TextMeshProUGUI pokeStatNameText in pokeStatsNameTextArray)
        {
            pokeStatNameText.text = "";
        }


        StartCoroutine(GetPokemonWithName(pokemonSearchName.GetComponent<TMP_InputField>().text.ToString()));
    }

    IEnumerator GetPokemonWithName(string pokemonName)
    {
        
        // Get Pokemon Info
        string pokemonURL = basePokeURL + "pokemon/" + pokemonName.ToString().ToLower();
        // Example URL: https://pokeapi.co/api/v2/pokemon/151

        Debug.Log(pokemonURL);

        //Creating a new web request using the get.
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);

        //Send web request to URL and return to ienumerator
        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }


        // Here we the pokemon info is pulled and parsed through the json. the text is returned as plain utf8 text.
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);

        //start assigning the text using information from the parsed data.
        string pokeName = pokeInfo["name"];
        string pokeNum = pokeInfo["id"];
        pokeNumText.text = pokeNum.ToString();


        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        
        //Set to root node
        JSONNode pokeTypes = pokeInfo["types"];
        //Create a single or Double element String Array. Assign new string array of size Poketypes.Count. We dont know
        //If there will be one or two types.
        string[] pokeTypeNames = new string[pokeTypes.Count];

        //Start i at 0. Start j at the end. Loop while I is less then the 
        for (int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeNames[j] = pokeTypes[i]["type"]["name"];
        }

        JSONNode pokeStats = pokeInfo["stats"];
        //Initialize a new string array. We dont know how large the array is yet, so we have to check.
        // Set the new string array size to .COUNT. The new string will assign the amount of values in the array.
        string[] pokeStatNames = new string[pokeStats.Count];
        

        for (int i = 0, j = pokeStats.Count - 1; i < pokeStats.Count; i++, j--)
        {
            pokeStatNames[j] = pokeStats[i]["stat"]["name"];
        }

        JSONNode pokeBaseValues = pokeInfo["stats"];
        string[] pokeBaseValuesNumber = new string[pokeBaseValues.Count];

        for(int i = 0, j = pokeBaseValues.Count - 1; i < pokeBaseValues.Count; i++, j--)
        {
            pokeBaseValuesNumber[j] = pokeBaseValues[i]["base_stat"];
        }
        


        // Get Pokemon Sprite information
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        yield return pokeSpriteRequest.SendWebRequest();
        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }


        // Set UI Objects
        pokeRawImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        pokeRawImage.texture.filterMode = FilterMode.Point;
        pokeNameText.text = CapitalizeFirstLetter(pokeName);
        for (int i = 0; i < pokeTypeNames.Length; i++)
        {
            pokeTypeTextArray[i].text = CapitalizeFirstLetter(pokeTypeNames[i]);
        }

        for (int i = 0; i < pokeStatNames.Length; i++)
        {
            pokeStatsNameTextArray[i].text = pokeStatNames[i];
        }

        for (int i = 0; i < pokeBaseValuesNumber.Length; i++)
        {
            pokeBaseValuesTextArray[i].text = pokeBaseValuesNumber[i];
        }

    }


    private string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }
}
