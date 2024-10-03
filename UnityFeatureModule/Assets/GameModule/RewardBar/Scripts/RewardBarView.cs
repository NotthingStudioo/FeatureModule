namespace GameModule.RewardBar.Scripts
{
    using System.Threading.Tasks;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class RewardBarView : MonoBehaviour
    {
        public                    RectTransform   cursor;
        public                    RectTransform   rewardBar; 
        public                    Button          stopButton; 
        public                    TextMeshProUGUI txtReward;

        [Inject] private readonly DOTween         doTween;
        private const float CursorSpeed   = 1000f;
        private       bool  isMovingRight = true;
        private       bool  isStopped     ;

        // Positions cursor mark
        private readonly float[] rewardPositions = { -300f, -200f, -100f, 0f, 100f, 200f, 300f };
        // Multipliers for mark
        private readonly int[] rewardMultipliers = { 2, 3, 4, 5, 4, 3, 2 };

        private void Start()
        {
            //Check the mark
            if (this.rewardPositions.Length != this.rewardMultipliers.Length)
            {
                Debug.LogError("rewardPositions != rewardMultipliers");
                return;
            }
            this.stopButton.onClick.AddListener(this.StopCursor);
            //Effect
            this.stopButton.transform.DOShakePosition(10f, 0.5f, 10)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void Update()
        {
            if (this.isStopped) return;
            this.MoveCursor();
            this.UpdateMultiplierText();
        }
        
        private void MoveCursor()
        {
            this.isMovingRight = this.isMovingRight switch
            {
                true when this.cursor.anchoredPosition.x >= this.rewardBar.rect.width / 2 => false,
                false when this.cursor.anchoredPosition.x <= -this.rewardBar.rect.width / 2 => true,
                _ => this.isMovingRight
            };
            var direction = this.isMovingRight ? 1f : -1f;
            this.cursor.anchoredPosition += new Vector2(direction * CursorSpeed * Time.deltaTime, 0);
        }
        
        private void UpdateMultiplierText()
        {
            var closestPosition = float.MaxValue;
            var rewardIndex     = 0;
            
            for (var i = 0; i < this.rewardPositions.Length; i++)
            {
                var distance = Mathf.Abs(this.cursor.anchoredPosition.x - this.rewardPositions[i]);

                if (!(distance < closestPosition)) continue;
                closestPosition = distance;
                rewardIndex     = i;
            }
            this.txtReward.text = "x" + this.rewardMultipliers[rewardIndex];
        }
        
        private async void StopCursor()
        {
            this.isStopped = true;
            this.UpdateMultiplierText();
            await Task.Delay(1000);
            
        }
    }
}