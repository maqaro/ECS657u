using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new List<TKey>();
    [SerializeField] private List<TValue> values = new List<TValue>();
    
    public void OnBeforeSerialize()
    // This method is called before the object is serialized
    {
        // Clear the lists before adding the keys and values
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        // Loop through the dictionary and add the keys and values to the lists
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    // This method is called after the object has been deserialized
    {
        this.Clear();

        for (int i = 0; i < keys.Count; i++)
        {
            // Add the keys and values back to the dictionary
            this.Add(keys[i], values[i]);
        }
    }
}
