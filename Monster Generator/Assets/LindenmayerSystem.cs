using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class LindenmayerSystem : MonoBehaviour
    {
        Dictionary<char, Rule> ruleset;

        public LindenmayerSystem()
        {
            
        }

        private void Awake()
        {
            ruleset = new Dictionary<char, Rule>();

            CreateRuleset(1, 2, 2);
            RunSystem("d", 6);
        }

        public void CreateRuleset(int heads, int legs, int arms)
        {
            StochasticRule rule = new StochasticRule('d');
            rule.AddPossibility(15, "bd");
            rule.AddPossibility(35, "ah");
            rule.AddPossibility(25, "hh");

            Rule rule2 = new Rule('b', "a");
            Rule rule3 = new Rule('a', "ad");

            StochasticRule rule4 = new StochasticRule('h');
            rule4.AddPossibility(35, "b");
            rule4.AddPossibility(65, "a");

            ruleset.Add(rule.input, rule);
            ruleset.Add(rule2.input, rule2);
            ruleset.Add(rule3.input, rule3);
            ruleset.Add(rule4.input, rule4);

            //Debug.Log(rule.GiveResult());
        }

        public string RunSystem(string start, int iterations)
        {
            string result = start;

            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < result.Length; j++)
                {
                    Rule rule = null;

                    if (ruleset.TryGetValue(result[j], out rule))
                    {
                        string output = rule.GiveResult();
                        result = result.Remove(j, 1).Insert(j, output);
                        j += output.Length - 1;
                    }
                    else
                        throw new Exception("This character does not exist in the ruleset");
                }

                Debug.Log(result);
            }

            return result;
        }
    }
}