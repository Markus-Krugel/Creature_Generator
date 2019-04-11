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
            ruleset = new Dictionary<char, Rule>();
        }

        private void Awake()
        {
            ruleset = new Dictionary<char, Rule>();

            CreateRuleset(1, 2, 2);
            RunSystem("L", 6);
        }

        public void CreateRuleset(int heads, int legs, int arms)
        {
            //Rule bodyRule = new Rule('B', "M");
            //
            //StochasticRule metaballRule = new StochasticRule('M');
            //metaballRule.AddPossibility(15, "M");
            //metaballRule.AddPossibility(20, "MCM");
            //metaballRule.AddPossibility(30, "MCCM");
            //
            //Rule positionRule = new Rule('C', "C");
            //
            //StochasticRule headRule = new StochasticRule('H');
            //headRule.AddPossibility(50, "CM");
            //headRule.AddPossibility(25, "CCM");
            //
            //StochasticRule legRule = new StochasticRule('L');
            //legRule.AddPossibility(35, "CM");
            //legRule.AddPossibility(65, "CCM");
            //
            //Rule armRule = new Rule('A', "PM");
            //
            //Rule positionRule = new Rule('P')
            //
            //ruleset.Add(bodyRule.input, bodyRule);
            //ruleset.Add(metaballRule.input, metaballRule);
            //ruleset.Add(positionRule.input, positionRule);

            Rule startRule = new Rule('L', "UM");
            Rule legRule = new Rule('U', "UM");
            Rule legRule2 = new Rule('M', "UM");

            ruleset.Add(startRule.input, startRule);
            ruleset.Add(legRule.input, legRule);
            ruleset.Add(legRule2.input, legRule2);


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

                        // Replaces the character with the output of the rule
                        result = result.Remove(j, 1).Insert(j, output);

                        // Adds to the counter so that you do not replace characters just added
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