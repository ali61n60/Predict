using Predict.Models;

namespace Predict.Policy
{
    public class SimplePolicy : IPolicy
    {
        private int _hostGoals;
        private int _guestGoals;

        public SimplePolicy(int hostGoals, int guestGoals)
        {
            _hostGoals = hostGoals;
            _guestGoals = guestGoals;
            Name = "simplePolicy(" + _hostGoals + "," + _guestGoals + ")";
        }
        public Prediction PredictMatch(Team hostTeam, Team guestTeam, int week)
        {
            return new Prediction()
            {
                HostGoals = _hostGoals,
                GuestGoals = _guestGoals
            };
        }

        public string Name { get; }
    }
}
