using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class LindenmayerSystem : MonoBehaviour
    {
        const float pairLegs = 2.0f;

        Dictionary<char, Rule> ruleset;

        // The maximum of iterations for each body part
        const int maxHeadIterations = 3;
        const int maxLegIterations = 4;
        const int maxArmIterations = 2;
        const int bodyHeigth = 6;
        // 2 for actual head + free space
        const int headWidth = 4;
        // 1 for actual arm + free space
        const int armWidth = 3;
        // each body area is a 3x3 cube
        const int bodySize = 3;

        int amountArms;
        int amountLegs;
        int amountHeads;

        /* These values keep track how many iterations each body part has currently
         * so that you can end the body parts after maximum of iterations for the part reached
         * and to fit the iterations of the body on the part based on the iteration count
         */
        int[] headIterations;
        int[] legIterations;
        int[] leftArmIterations;
        int[] rightArmIterations;
        int[] bodyIterations;

        bool[] legsDone;

        public LindenmayerSystem()
        {
            ruleset = new Dictionary<char, Rule>();
            CreateRuleset();
        }

        public void CreateRuleset()
        {
            // Leg rule

            StochasticRule legRule = new StochasticRule('L');
            //metaball and up
            legRule.AddPossibility(25, "UML");
            // metaball middle,forward and backwards
            legRule.AddPossibility(25, "UMFNNMFL");
            // metaballs in a diamond form
            legRule.AddPossibility(25, "UFMNPMRNMFML");

            Possibility endLegPossibility = new Possibility(15, "MUB");
            legRule.AddPossibility(endLegPossibility);
            legRule.SetEndPossibility(endLegPossibility);

            // body rule

            //add metaballs in a 3x3 cube with another iteration for body
            Rule bodyRule = new Rule('B', "MFMRMNMNMPMPMFMFMRNUB");
            //add metaballs in a 3x3 cube, ends the body in this area and reset the position
            Possibility endBodyPossibility = new Possibility(0, "MFMRMNMNMPMPMFMFMQ");
            bodyRule.SetEndPossibility(endBodyPossibility);

            // left arm rule

            StochasticRule leftArmRule = new StochasticRule('A');
            leftArmRule.AddPossibility(20, "MRA");
            leftArmRule.AddPossibility(20, "MUMRMRA");
            leftArmRule.AddPossibility(20, "MDMRMRA");

            Possibility endLeftArmPossibility = new Possibility(20, "M");
            leftArmRule.SetEndPossibility(endLeftArmPossibility);

            // right arm rule

            StochasticRule rightArmRule = new StochasticRule('V');
            rightArmRule.AddPossibility(20, "MPV");
            rightArmRule.AddPossibility(20, "MUMPMPV");
            rightArmRule.AddPossibility(20, "MDMPMPV");

            Possibility endRightArmPossibility = new Possibility(20, "M");
            rightArmRule.SetEndPossibility(endLeftArmPossibility);

            // head rule

            Rule headRule = new Rule('H',"MRMUPH");

            Possibility endHeadPossibility = new Possibility(20, "MRM");
            headRule.SetEndPossibility(endHeadPossibility);

            // Add the created rules to the ruleset
            ruleset.Add(legRule.input, legRule);
            ruleset.Add(bodyRule.input, bodyRule);
            ruleset.Add(leftArmRule.input, leftArmRule);
            ruleset.Add(rightArmRule.input, rightArmRule);
            ruleset.Add(headRule.input, headRule);
        }

        public void SetBodyPartsAmount(int heads,int arms, int legs)
        {
            // Set the array size depending on the amount for each body part
            headIterations = new int[heads];
            legIterations = new int[legs];
            bodyIterations = new int[legs];
            leftArmIterations = new int[arms];
            rightArmIterations = new int[arms];
            legsDone = new bool[legs];

            amountLegs = legs;
            amountHeads = heads;
            amountArms = arms;
        }

        /// <summary>
        /// Adds arms and heads to the body. Expands the body if not enough space available.
        /// </summary>
        /// <returns>The string to add arms and heads to the body</returns>
        private string AddArmsAndHeads()
        {
            StringBuilder toAdd = new StringBuilder();

            // reset Z Position
            toAdd.Append("Z");

            int additionalBodyIterationsZ = 0;
            int additionalBodyIterationsX = 0;

            int bodyWidthZ = (int)(amountLegs / pairLegs) * bodySize;
            int neededArmWidth = amountArms * armWidth;

            if (neededArmWidth > bodyWidthZ)
            {
                int armDifference = neededArmWidth - bodyWidthZ;
                // Multiplied by 1.0f so that it is a float and therefore no error
                additionalBodyIterationsZ = (int)Math.Ceiling(armDifference / bodySize * 1.0f);
            }

            int bodyWidthX = bodySize;

            if (amountLegs > 1)
                bodyWidthX = bodySize * 2;

            int neededHeadWith = amountHeads * headWidth;

            if (neededHeadWith > bodyWidthX)
            {
                int headDifference = neededHeadWith - bodyWidthX;
                // Multiplied by 1.0f so that it is a float and therefore no error
                additionalBodyIterationsX = (int)Math.Ceiling(headDifference / bodySize * 1.0f);
            }

            // Set to the correct heigth
            toAdd.Append("UUUUU");

            // if odd then left side gets the additional one
            int iterationsLeftSide = (int)Math.Ceiling(additionalBodyIterationsX / 2 * 1.0f);
            int iterationsRightSide = additionalBodyIterationsX / 2;

            // Adds additonal body parts for the heads
            if (additionalBodyIterationsX != 0)
            {
                // single leg has one less width as the left leg in pairs is further left
                if (amountLegs == 1)
                    toAdd.Append("PPP");
                else
                    toAdd.Append("PPPP");

                // add to the left side
                for (int i = 0; i < iterationsLeftSide; i++)
                {
                    toAdd.Append("BUBDPP");
                }

                // reset x position
                toAdd.Append("X");

                // single leg has one less width as the right leg in pairs is further right
                if (amountLegs == 1)
                    toAdd.Append("RRR");
                else
                    toAdd.Append("RRRR");

                // add to the right side
                for (int i = 0; i < iterationsRightSide; i++)
                {
                    toAdd.Append("BUBDRR");
                }
            }

            // Adds additional body parts for the arms
            if(additionalBodyIterationsZ != 0)
            {
                string direction = "RR";

                // When we added addtional body areas on the x side then we want to start from last position there
                if (additionalBodyIterationsX != 0)
                {
                    // decides the direction based on which side we ended at additionalBodyIterationsX
                    if (iterationsLeftSide != iterationsRightSide)
                    {
                        direction = "PP";
                    }
                }
                else
                {
                    // single leg has one less width as the left leg in pairs is further left
                    if (amountLegs == 1)
                        toAdd.Append("NNNN");
                    else
                        toAdd.Append("PNNNN");
                }

                int sidewaysIteration;

                // when only one leg then there is only one body area added else two
                if (amountLegs == 1)
                    sidewaysIteration = 1 + additionalBodyIterationsX;
                else
                    sidewaysIteration = 2 + additionalBodyIterationsX;

                for (int i = 0; i < sidewaysIteration; i++)
                {
                    for (int j = 0; j < additionalBodyIterationsZ; j++)
                    {
                        // add the body areas
                        toAdd.Append("BUBD");

                        // move in alternating directions
                        if (i % 2 == 0)
                            toAdd.Append("NN");
                        else
                            toAdd.Append("FF");
                    }
                    toAdd.Append(direction);
                }

            }

            toAdd.Append("XZU");

            // add arms to the left side

            // single leg has one less width as the left leg in pairs is further left
            if (amountLegs == 1)
                toAdd.Append("FRRR");
            else
                toAdd.Append("NRRR");

            for (int i = 0; i < iterationsLeftSide; i++)
            {
                toAdd.Append("RR");
            }

            for (int i = 0; i < amountArms; i++)
            {
                toAdd.Append("RAP");
                toAdd.Append('N', armWidth);
            }

            toAdd.Append("QUUUUU");

            // add arms to the right side

            // single leg has one less width as the right leg in pairs is further tight
            if (amountLegs == 1)
                toAdd.Append("FRRR");
            else
                toAdd.Append("NPP");

            for (int i = 0; i < iterationsRightSide; i++)
            {
                toAdd.Append("PP");
            }

            for (int i = 0; i < amountArms; i++)
            {
                toAdd.Append("PVR");
                toAdd.Append('N', armWidth);
            }

            toAdd.Append("QUUUUU");

            // add heads

            if (amountLegs == 1)
                toAdd.Append("FRRR");
            else
                toAdd.Append("NPP");

            for (int i = 0; i < additionalBodyIterationsZ; i++)
            {
                toAdd.Append("NN");
            }

            for (int i = 0; i < amountHeads; i++)
            {
                toAdd.Append("NHF");
                toAdd.Append('R', headWidth);
            }

            return toAdd.ToString();
        }

        private string AddLegs()
        {
            StringBuilder start = new StringBuilder();

            // The amount of iterations to add legs in pairs, rounds up so that a single leg can be added
            int iterationsLeg = (int)Math.Ceiling(amountLegs / pairLegs);

            for (int i = 0; i < iterationsLeg; i++)
            {
                // Add a single leg if odd amount of legs and last iteration
                if (i == iterationsLeg - 1 && amountLegs % pairLegs == 1)
                    start.Append("ML");
                // Add legs in pairs(left,leg,resetX,Right,leg,forward,forward)
                else
                    start.Append("PMLRRMLFF");
            }
            return start.ToString();
        }

        public string RunSystem(int iterations)
        {
            string result = AddLegs();

            int headsIndex = 0;
            int rightArmsIndex = 0;
            int leftArmsIndex = 0;
            int legsIndex = 0;
            int bodyIndex = 0;

            bool bodyDone = false;
            bool addedArmsAndHeads = false;

            int interationBodyDone = 999;

            for (int i = 0; i < iterations; i++)
            {
                // Reset the indexes every iteration
                headsIndex = 0;
                rightArmsIndex = 0;
                leftArmsIndex = 0;
                legsIndex = 0;
                bodyIndex = 0;

                for (int j = 0; j < result.Length; j++)
                {
                    Rule rule = null;

                    if (ruleset.TryGetValue(result[j], out rule))
                    {

                        bool maxIterationsReached = false;

                        /* Checks which body part currently is as char.
                         * After that checks if the max iterations of the specific part is reached.
                         * Finally adds to the index.
                         */
                        switch (result[j])
                        {
                            case 'L':
                                //skips ended legs parts, for example if leg 2 is done it would skip to the third leg
                                while (legsDone[legsIndex])
                                {
                                    legsIndex++;
                                }

                                if (legIterations[legsIndex] == maxLegIterations)
                                    maxIterationsReached = true;
                                break;
                            case 'A':
                                if (leftArmIterations[leftArmsIndex] == maxArmIterations)
                                    maxIterationsReached = true;
                                break;
                            case 'V':
                                if (rightArmIterations[rightArmsIndex] == maxArmIterations)
                                    maxIterationsReached = true;
                                break;
                            case 'H':
                                if (headIterations[headsIndex] == maxHeadIterations)
                                    maxIterationsReached = true;
                                break;
                            case 'B':
                                if (!addedArmsAndHeads)
                                {
                                    if (bodyIterations[bodyIndex] + legIterations[bodyIndex] == bodyHeigth)
                                    {
                                        maxIterationsReached = true;
                                        bodyDone = true;
                                    }
                                }
                                break;
                        }

                        Debug.Log(result);
                        // If max iterations reached then end the body part
                        string output;


                        if (maxIterationsReached)
                        {
                            // set iteration value of body part to -1 if endresult, so that it skips its index next time
                            output = rule.GiveEndString();
                        }
                        // For the body expansion of arms and heads added
                        else if (addedArmsAndHeads && (result[j] == 'B'))
                        {
                            output = rule.GiveEndString();
                        }
                        else
                        {
                            output = rule.GiveResult();
                            if (output == rule.GiveEndString() && result[j] == 'L')
                                legsDone[legsIndex] = true;
                        }

                        switch (result[j])
                        {
                            case 'L':
                                legIterations[legsIndex]++;
                                legsIndex++;
                                break;
                            case 'A':
                                leftArmIterations[leftArmsIndex]++;
                                leftArmsIndex++;
                                break;
                            case 'V':
                                rightArmIterations[rightArmsIndex]++;
                                rightArmsIndex++;
                                break;
                            case 'H':
                                headIterations[headsIndex]++;
                                headsIndex++;
                                break;
                            case 'B':
                                if (!addedArmsAndHeads)
                                {
                                    bodyIterations[bodyIndex]++;
                                    bodyIndex++;
                                }
                                break;
                        }
                        // Replaces the character with the output of the rule
                        result = result.Remove(j, 1).Insert(j, output);

                        // Adds to the counter so that you do not replace characters just added
                        j += output.Length - 1;
                    } 
                }  
                
                if(bodyDone)
                {
                    result += AddArmsAndHeads();
                    bodyDone = false;
                    addedArmsAndHeads = true;

                    interationBodyDone = i;
                }

                if (i == interationBodyDone + 1)
                    addedArmsAndHeads = false;
            }
            Debug.Log(result);
            return result;
        }
    }
}