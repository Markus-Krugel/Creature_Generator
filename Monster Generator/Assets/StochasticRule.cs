using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Assets
{
    public class Possibility
    {
        public float percentage;
        public string output;

        public Possibility(float percentage, string output)
        {
            this.percentage = percentage;
            this.output = output;
        }
    }

    class StochasticRule : Rule
    {
        float sumPercentages;
        List<Possibility> resultPossibilities;

        public StochasticRule(char input)
        {
            this.input = input;
            resultPossibilities = new List<Possibility>();
        }

        public StochasticRule(char input, List<Possibility> resultPossibilities)
        {
            this.input = input;
            this.resultPossibilities = resultPossibilities;
        }

        public void AddPossibility(Possibility possibility)
        {
            resultPossibilities.Add(possibility);
        }

        public void AddPossibility(float percentage, string output)
        {
            resultPossibilities.Add(new Possibility(percentage, output));        
        }

        private void NormalizePercentages()
        {
            sumPercentages = 0;
            foreach (Possibility possibility in resultPossibilities)
                sumPercentages += possibility.percentage;

            float factor = 1 / sumPercentages;

            for (int i = 0; i < resultPossibilities.Count; i++)
                resultPossibilities[i].percentage *= factor;
        }

        public void showAllPossibilities()
        {
            foreach (Possibility possibility in resultPossibilities)
                Debug.Log(possibility.percentage + ", " + possibility.output);
        }

        public override string GiveResult()
        {
            if (resultPossibilities.Count != 0)
            {
                NormalizePercentages();

                Random random = new Random();
                float randomResult = (float)random.NextDouble();

                float currentPercentage = 0;

                for (int i = 0; i < resultPossibilities.Count; i++)
                {
                    if (randomResult < resultPossibilities[i].percentage + currentPercentage)
                        return resultPossibilities[i].output;
                    else
                        currentPercentage += resultPossibilities[i].percentage;
                }
            }
            else
                throw new Exception("This stochastic rule does not have any possibilities");

            return "";
        }
    }
}
