namespace Koakuma.Game.Skills
{
    public static class SkillFactory
    {
        public static GameSkillComponent CreateSkillEntity(CastSkillData castSkillData, SkillData skillData)
        {
            GameSkillEntity gameSkillEntity = new GameSkillEntity();
            return gameSkillEntity.AddComponent<GameSkillComponent, CastSkillData, SkillData>(castSkillData, skillData);
        }
    }
}
