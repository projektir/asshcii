namespace asshcii.game.actions
{
    public class UpgradeBuildingAction : IAction
    {
        public Building Building { get; }

        public UpgradeBuildingAction(Building building)
        {
            Building = building;
        }
        public ExecuteResult Execute(PlayerBase playerBase)
        {
            var costs = Building.GetComponent<Resources>();
            var resources = playerBase.Resources;

            if (!resources.TrySubtractResources(costs, out resources))
            {
                return new NotEnoughResources();
            }
            Building.Upgrade();

            if (!playerBase.Buildings.Contains(Building))
            {
                playerBase.Buildings.Add(Building);
            }

            playerBase.Resources = resources;

            return new Success();
        }
    }

    public class NotEnoughResources : ExecuteResult {
        public NotEnoughResources() : base(false, "Not enough resources") {
        }
    }
}