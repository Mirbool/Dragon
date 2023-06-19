namespace Koakuma.Game.Inputs
{
    public class InputConst
    {
        public enum ActionID
        {
            GamePlay_MoveHorizontal = 0,
            GamePlay_MoveVertical = 1,
            GamePlay_Attack = 2,
            GamePlay_Skill1 = 3,
            GamePlay_Skill2 = 4,
            GamePlay_Skill3 = 5,
            GamePlay_Skill4 = 6,
            GamePlay_Dash = 7,
            //GamePlay_Menu = 6,
            //GamePlay_Interactive1 = 11,
            //GamePlay_Interactive2 = 12,
            //UI_Up = 13,
            //UI_Down = 14,
            //UI_Left = 15,
            //UI_Right = 16,
            //UI_Close = 17,
        }

        public enum CategoryID
        {
            GamePlay = 0,
            UI = 2,
        }
    }
}
