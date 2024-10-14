namespace Game.Scripts.Shop
{
    using System.Collections.Generic;
    using System.Linq;
    using FeatureTemplate.Scripts.RewardHandle;
    using Game.Scripts.Blueprints;
    using Game.Scripts.Shop.Condition;
    using Game.Scripts.Shop.Cost;

    public class FeatureTransactionService
    {
        private readonly List<ICost>                      costs;
        private readonly List<IShopCondition>             conditions;
        private readonly List<IFeatureRewardExecutorBase> rewardExecutors;

        public FeatureTransactionService(List<ICost> costs, List<IShopCondition> conditions, List<IFeatureRewardExecutorBase> rewardExecutors)
        {
            this.costs           = costs;
            this.conditions      = conditions;
            this.rewardExecutors = rewardExecutors;
        }

        public bool CheckCondition(ITransactionRecord transactionRecord)
        {
            // Get the list of required conditions from the transactionRecord
            var listRequiredConditions = transactionRecord.GetConditions();

            // Check if all required conditions are met
            return listRequiredConditions.All(requiredCondition =>
            {
                // Find a matching condition in `this.conditions` by ConditionType
                var matchingCondition = this.conditions.FirstOrDefault(condition =>
                    condition.Id.Equals(requiredCondition.ConditionType));

                // If a matching condition is found, check if it's met using the required ConditionParam
                return matchingCondition != null && matchingCondition.IsMet(requiredCondition.ConditionParam);
            });
        }

        public bool DoPurchase(ITransactionRecord transactionRecord)
        {
            // if any item can not afford, return false
            if ((from item in this.costs let matchingCost = transactionRecord.GetCosts().
                        FirstOrDefault(cost => cost.CostType.Equals(item.Id)) 
                    where matchingCost != null 
                    where !item.CanAfford(matchingCost) 
                    select item).Any())
            {
                return false;
            }
            
            // Check all if any cost cannot purchase
            foreach (var item in this.costs)
            {
                // iterate through the list of costs and find the matching cost by CostId
                var matchingCost = transactionRecord.GetCosts().FirstOrDefault(cost => cost.CostType.Equals(item.Id));

                if (matchingCost == null)
                    continue;

                // If a matching cost is found, try purchase it
                item.Purchase(matchingCost);
            }

            foreach (var item in this.rewardExecutors)
            {
                var matchingDeliverable = transactionRecord.GetDeliverables().FirstOrDefault(deliverable => deliverable.RewardType == item.RewardType);

                if(matchingDeliverable == null)
                    continue;
                
                item.ReceiveReward(matchingDeliverable, null);
            }

            return true;
        }
    }
}