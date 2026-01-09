using Handscribed.Dataset;
using System;
using System.Collections.Generic;
using System.Text;

namespace Handscribed
{
    public interface IRepository
    {
        IList<MnistImage> TrainingData { get; }
    }

    public interface IRepositoryInitializer : IRepository
    {
        void SetTrainingData(IList<MnistImage> data);
    }
    internal class Repository : IRepositoryInitializer
    {
        private IList<MnistImage> _trainingData;

        public IList<MnistImage> TrainingData => _trainingData;

        public void SetTrainingData(IList<MnistImage> data)
        {
            _trainingData = data;
        }
    }
}
