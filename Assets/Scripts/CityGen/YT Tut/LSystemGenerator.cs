using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentence;
    [Range(0,10)]
    public int iterationLimit = 1;

    public bool randomlyIgnoreRules = true;
    [Range(0f,1f)]
    public float randomIgnoreChance = .3f;

    private void Start()
    {
        //Debug.Log(GenerateSentence2());
    }

    public string GenerateSentence2()
    {
        string prevIterationResult = rootSentence;
        for(int i = 0; i < iterationLimit; i++)
        {
            StringBuilder newString = new StringBuilder();
            foreach(char c in prevIterationResult)
            {
                Rule rule = rules.FirstOrDefault(r => r.letter.Equals(c.ToString()));
                if(rule != null){
                    newString.Append(rule.GetResult());
                }
                else
                {
                    newString.Append(c);
                }
            }
            prevIterationResult = newString.ToString();
        }
        return prevIterationResult.ToString();
    }

    public string GenerateSentence(string word = null)
    {
        if(word == null)
        {
            word = rootSentence;
        }
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int iteration = 0)
    {
        if(iteration >= iterationLimit)
        {
            return word;
        }
        StringBuilder newWord = new StringBuilder();
        foreach(var c in word)
        {
            newWord.Append(c);
            foreach(var rule in rules)
            {
                if (randomlyIgnoreRules)
                {
                    float random = UnityEngine.Random.Range(0f, 1f);
                    if (random < randomIgnoreChance && iteration > 1)
                    {
                        return newWord.ToString();
                    }
                }
                if (rule.letter.Equals(c.ToString()))
                {
                    newWord.Append(GrowRecursive(rule.GetResult(), iteration + 1));
                }
            }
        }

        return newWord.ToString();
    }
}
