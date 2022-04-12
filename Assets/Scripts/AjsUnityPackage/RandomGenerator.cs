using System.Collections.Generic;
using UnityEngine;

namespace AjsUtilityPackage
{
    public class RandomGenerator : MonoBehaviour
    {
        public static float GetRandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }

        public static int GetRandomInt(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        public static bool GetRandomBool()
        {
            int coinFlip = GetRandomInt(0, 1);

            if (coinFlip == 0) return true;
            else return false;
        }

        public static string GetRandomString(string[] strings)
        {
            return strings[GetRandomInt(0, strings.Length - 1)];
        }

        public static string GetRandomString(List<string> strings)
        {
            return strings[GetRandomInt(0, strings.Count - 1)];
        }

        public static GameObject GetRandomGameObject(GameObject[] gameObjects)
        {
            return gameObjects[GetRandomInt(0, gameObjects.Length - 1)];
        }

        public static GameObject GetRandomGameObject(List<GameObject> gameObjects)
        {
            return gameObjects[GetRandomInt(0, gameObjects.Count - 1)];
        }

        public static Color32 GetRandomColor()
        {
            byte r = (byte)GetRandomFloat(0, 255);
            byte g = (byte)GetRandomFloat(0, 255);
            byte b = (byte)GetRandomFloat(0, 255);
            return new Color32(r, g, b, 255);
        }

        public static Color32 GetRandomColor(float alpha)
        {
            byte r = (byte)GetRandomFloat(0, 255);
            byte g = (byte)GetRandomFloat(0, 255);
            byte b = (byte)GetRandomFloat(0, 255);

            alpha = Mathf.Clamp(alpha, 0f, 225f);
            byte a = (byte)alpha; 

            return new Color32(r, g, b, a);
        }
    }
}