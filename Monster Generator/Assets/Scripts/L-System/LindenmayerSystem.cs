using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class LindenmayerSystem : MonoBehaviour
    {
        Dictionary<char, Rule> ruleset;

        const float PAIR_LEGS = 2.0f;

        // The maximum of iterations for each body part
        const int MAX_HEAD_ITERATIONS = 3;
        const int MAX_LEGS_ITERATIONS = 4;
        const int MAX_ARMS_ITERATIONS = 2;
        const int BODY_HEIGTH = 6;
        // 2 for actual head + free space
        const int HEAD_WIDTH = 4;
        // 1 for actual arm + free space
        const int ARM_WIDTH = 3;
        // each body area is a 3x3 cube
        const int BODY_SIZE = 3;

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

        //stores the commands of the body parts to reverse them later to avoid misplaced body parts 
        string[] armsLeftString;
        string[] armsRightString;
        string[] headsString;

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
            legRule.AddPossibility(40, "UML");
            // metaball middle,forward and backwards
            legRule.AddPossibility(30, "UMFMNNMFL");
            // metaballs in a diamond form
            legRule.AddPossibility(30, "UFMNPMRNMFL");

            Possibility endLegPossibility = new Possibility(15, "MUB");
            legRule.AddPossibility(endLegPossibility);
            legRule.SetEndPossibility(endLegPossibility);

            // body rule

            //add metaballs in a 3x3 cube with another iteration for body
            Rule bodyRule = new Rule('B', "MFMRMNMNMPMPMFMFMRNUB");
            //add metaballs in a 3x3 cube, ends the body in this area and reset the position
            Possibility endBodyPossibility = new Possibility(0, "MFMRMNMNMPMPMFMFMRNQ");
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

            Possibility endHeadPossibility = new Possibility(0, "MRMP");
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

            armsLeftString = new string[arms];
            armsRightString = new string[arms];
            headsString = new string[heads];

            legsDone = new bool[legs];

            amountLegs = legs;
            amountHeads = heads;
            amountArms = arms;
        }      

        /// <summary>
        /// Start the string by adding legs depending on legs amount
        /// </summary>
        /// <returns></returns>
        private string AddLegs()
        {
            StringBuilder start = new StringBuilder();

            // The amount of iterations to add legs in pairs, rounds up so that a single leg can be added
            int iterationsLeg = (int)Math.Ceiling(amountLegs / PAIR_LEGS);

            for (int i = 0; i < iterationsLeg; i++)
            {
                // Add a single leg if odd amount of legs and last iteration
                if (i == iterationsLeg - 1 && amountLegs % PAIR_LEGS == 1)
                    start.Append("ML");
                // Add legs in pairs(left,leg,resetX,Right,leg,forward,forward)
                else
                    start.Append("PMLRRMLPFF");
            }
            return start.ToString();
        }

        /// <summary>
        /// Adds arms and heads to the body. Expands the body if not enough space available.
        /// </summary>
        /// <returns>The string to add arms and heads to the body</returns>
        private string AddArmsAndHeads()
        {
            StringBuilder toAdd = new StringBuilder();

            // reset Z Position
            toAdd.Append("Q");

            int additionalBodyIterationsZ = 0;
            int additionalBodyIterationsX = 0;

            int bodyWidthZ = (int)(amountLegs / PAIR_LEGS) * BODY_SIZE;
            int neededArmWidth = amountArms * ARM_WIDTH;

            if (neededArmWidth > bodyWidthZ)
            {
                int armDifference = neededArmWidth - bodyWidthZ;
                additionalBodyIterationsZ = (int)Math.Ceiling((double)armDifference / BODY_SIZE);
            }

            int bodyWidthX = BODY_SIZE;

            if (amountLegs > 1)
                bodyWidthX = BODY_SIZE * 2;

            int neededHeadWith = amountHeads * HEAD_WIDTH;

            if (neededHeadWith > bodyWidthX)
            {
                int headDifference = neededHeadWith - bodyWidthX;
                additionalBodyIterationsX = (int)Math.Ceiling((double)headDifference / BODY_SIZE);
            }

            // Set to the correct heigth
            toAdd.Append("UUUUU");

            // if odd then left side gets the additional one
            int iterationsLeftSide = (int)Math.Ceiling((double)additionalBodyIterationsX / 2);
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
            if (additionalBodyIterationsZ != 0)
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
                toAdd.Append('N', ARM_WIDTH);
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
                toAdd.Append('N', ARM_WIDTH);
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
                toAdd.Append('R', HEAD_WIDTH);
            }

            return toAdd.ToString();
        }

        /// <summary>
        /// The main function of the class. Defines the shape of the creature by replacing chars of the string
        /// </summary>
        /// <param name="iterations">The number of iterations the system runs through</param>
        /// <returns>Returns the string which defines the shape of the creature</returns>
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

                    // Get the rule of the char if available
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

                                if (legIterations[legsIndex] == MAX_LEGS_ITERATIONS)
                                    maxIterationsReached = true;
                                break;
                            case 'A':
                                if (leftArmIterations[leftArmsIndex] == MAX_ARMS_ITERATIONS)
                                    maxIterationsReached = true;
                                break;
                            case 'V':
                                if (rightArmIterations[rightArmsIndex] == MAX_ARMS_ITERATIONS)
                                    maxIterationsReached = true;
                                break;
                            case 'H':
                                if (headIterations[headsIndex] == MAX_HEAD_ITERATIONS)
                                    maxIterationsReached = true;
                                break;
                            case 'B':
                                if (!addedArmsAndHeads)
                                {
                                    if (bodyIterations[bodyIndex] + legIterations[bodyIndex] == BODY_HEIGTH)
                                    {
                                        maxIterationsReached = true;
                                        bodyDone = true;
                                    }
                                }
                                break;
                        }

                        Debug.Log(result);
                        string output;

                        // If max iterations reached then end the body part
                        if (maxIterationsReached)
                        {
                            output = rule.GiveEndString();

                            if (result[j] == 'A')
                            {
                                armsLeftString[leftArmsIndex] += output;

                                char[] reverseCharArray = armsLeftString[leftArmsIndex].ToCharArray();
                                Array.Reverse(reverseCharArray);

                                string reverseString = new string(reverseCharArray);

                                // Replaces characters with opposite direction to move back
                                reverseString = reverseString.Replace("M", string.Empty);
                                reverseString = reverseString.Replace("A", string.Empty);
                                reverseString = reverseString.Replace('U', 'D');
                                reverseString = reverseString.Replace('D', 'U');
                                reverseString = reverseString.Replace('R', 'P');

                                output += reverseString;
                            }
                            if (result[j] == 'V')
                            {
                                armsRightString[rightArmsIndex] += output;

                                char[] reverseCharArray = armsRightString[rightArmsIndex].ToCharArray();
                                Array.Reverse(reverseCharArray);

                                string reverseString = new string(reverseCharArray);

                                // Replaces characters with opposite direction to move back
                                reverseString = reverseString.Replace("M", string.Empty);
                                reverseString = reverseString.Replace("V", string.Empty);
                                reverseString = reverseString.Replace('U', 'D');
                                reverseString = reverseString.Replace('D', 'U');
                                reverseString = reverseString.Replace('P', 'R');

                                output += reverseString;
                            }
                            if (result[j] == 'H')
                            {
                                headsString[headsIndex] += output;

                                char[] reverseCharArray = headsString[headsIndex].ToCharArray();
                                Array.Reverse(reverseCharArray);

                                string reverseString = new string(reverseCharArray);

                                // Replaces characters with opposite direction to move back
                                reverseString = reverseString.Replace("M", string.Empty);
                                reverseString = reverseString.Replace("H", string.Empty);
                                reverseString = reverseString.Replace('U', 'D');
                                reverseString = reverseString.Replace('D', 'U');
                                reverseString = reverseString.Replace('P', 'R');
                                reverseString = reverseString.Replace('R', 'P');

                                output += reverseString;
                            }
                        }
                        // For the body expansion of arms and heads added
                        else if (addedArmsAndHeads && result[j] == 'B')
                        {
                            // Add the end possibility of the body without the reset position at the end
                            output = rule.GiveEndString();
                            output = output.Remove(output.Length - 1);

                            // Add the output to the array to reverse it later
                            if (result[j] == 'A')
                                armsLeftString[leftArmsIndex] += output;
                            if (result[j] == 'V')
                                armsRightString[rightArmsIndex] += output;
                            if (result[j] == 'H')
                                headsString[headsIndex] += output;
                        }
                        else
                        {
                            output = rule.GiveResult();
                            if (output == rule.GiveEndString() && result[j] == 'L')
                                legsDone[legsIndex] = true;

                            // Add the output to the array to reverse it later
                            if (result[j] == 'A')
                                armsLeftString[leftArmsIndex] += output;
                            if (result[j] == 'V')
                                armsRightString[rightArmsIndex] += output;
                            if (result[j] == 'H')
                                headsString[headsIndex] += output;
                        }

                        // advances the according indexes and iterations
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
                
                // if the body is created add the arms and heads to the creature
                if(bodyDone)
                {
                    Rule bodyRule;
                    ruleset.TryGetValue('B', out bodyRule);
                    result = result.Replace("B", bodyRule.GiveEndString());

                   result += AddArmsAndHeads();
                   bodyDone = false;
                   addedArmsAndHeads = true;
                   
                   interationBodyDone = i;
                }

                // the addition of the arms and heads are done in the iteration after adding
                if (i == interationBodyDone + 1)
                    addedArmsAndHeads = false;
            }
            Debug.Log(result);
            return result;
        }
    }
}