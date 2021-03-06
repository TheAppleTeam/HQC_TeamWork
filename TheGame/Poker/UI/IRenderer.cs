﻿namespace Poker.UI
{
    using System.Windows.Forms;
    using GameObjects;
    using GameObjects.Cards;
    using GameObjects.Player;

    public interface IRenderer
    {
        void Clear();

        void Draw(IPlayer[] players);

        void Draw(IPlayer player);

        void Draw(params GameCard[] cards);
        
        void Draw(Table table);

        void ShowMessage(string msg);
        void ShowMessage(string msg, string msg1);

        void StartGamerTurnTimer();
        
        // да се провери необходимо ли е 
        void ManageGamersEntities(Gamer player);

        void EnablingFormMinimizationAndMaximization();
        
        // да се провери необходимо ли е 
        void StopGamerTurnTimer();

        // да се провери необходимо ли е 
        void ShowAllCards();

        // да се провери необходимо ли е 
        void SetAllLabelStatus(IPlayer[] players);

        void SetLabelStatus(IPlayer player, string labelText);

        void SetPanelStatus(Panel panel, bool isVisible);

        void SetTextBoxPlayerChips(IPlayer player);
        
        void GetCardsImages(GameCard[] gameDealtCards);

        void DisplayCardsOnTable();
    }
}
