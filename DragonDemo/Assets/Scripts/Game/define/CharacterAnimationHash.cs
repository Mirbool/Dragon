using UnityEngine;

public static class CharacterAnimationHash
{
    public static int PARAMETER_DYING = Animator.StringToHash("dying");
    public static int PARAMETER_DEAD = Animator.StringToHash("dead");
    public static int PARAMETER_SPEED = Animator.StringToHash("speed");
    public static int PARAMETER_LOCK_ON = Animator.StringToHash("lockon");
    public static int PARAMETER_LOCK_ON_SPEED_X = Animator.StringToHash("lockon_move_x");
    public static int PARAMETER_LOCK_ON_SPEED_Y = Animator.StringToHash("lockon_move_y");
    public static int PARAMETER_INTERRUPT = Animator.StringToHash("interrupt");
    public static int PARAMETER_STIFF = Animator.StringToHash("stiff");
    public static int PARAMETER_STIFF_FRONT = Animator.StringToHash("stiff_front");
    public static int PARAMETER_STIFF_SPEED = Animator.StringToHash("stiff_speed");
    public static int PARAMETER_STUN = Animator.StringToHash("stun");
    public static int PARAMETER_KNOCK_DOWN = Animator.StringToHash("knock_down");
    public static int PARAMETER_GET_UP = Animator.StringToHash("get_up");
    public static int PARAMETER_KNOCK_FLOAT = Animator.StringToHash("knock_float");
    public static int PARAMETER_KNOCK_FLOAT_SPEED = Animator.StringToHash("knock_float_speed");
    public static int PARAMETER_SKILL_CHARGE = Animator.StringToHash("skill_charge");
    public static int PARAMETER_DELIVER = Animator.StringToHash("deliver");
    public static int PARAMETER_DELIVER_BACK = Animator.StringToHash("deliver_back");
    public static int PARAMETER_RESCUE = Animator.StringToHash("rescue");
    public static int PARAMETER_HAS_WEAPON = Animator.StringToHash("has_weapon");
    public static int FREE = Animator.StringToHash("Free");
}
