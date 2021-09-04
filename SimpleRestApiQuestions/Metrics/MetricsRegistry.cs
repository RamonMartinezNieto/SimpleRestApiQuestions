using App.Metrics;
using App.Metrics.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleRestApiQuestions.Metrics
{
    public class MetricsRegistry
    {
        public static CounterOptions NewQuizStartCounter => new CounterOptions
        {
            Name = "Start Quiz",
            Context = "QuizController",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions CategoryQuizStart(int idCategory) => new CounterOptions
        {
            Name = $"Category Quiz Start {idCategory}",
            Context = "QuizController",
            MeasurementUnit = Unit.Calls
        };
    }
}
