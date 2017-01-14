using NosTayleGameServer.NosTale.Entities;
using NosTayleGameServer.NosTale.Maps;
using NosTayleGameServer.NosTale.Skills.SkillTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Skills
{
    static class SkillsManager
    {
        internal static Dictionary<int, Skill> skills;
        internal static Dictionary<int, Dictionary<int, int>> classSkills;
        internal static Dictionary<int, Dictionary<int, int>> spsSkills;

        static SkillsManager()
        {
            //SKILL INITIALIZATION
            SkillsManager.skills = new Dictionary<int, Skill>();
            SkillsManager.classSkills = new Dictionary<int, Dictionary<int, int>>();
            SkillsManager.spsSkills = new Dictionary<int, Dictionary<int, int>>();
            List<SkillBuff> buffs = new List<SkillBuff>();
            SkillsManager.skills.Add(200, new Skill(200, "Balancement", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 200", "11 200", "13 205" }, false, 1, 0, 0, 2, 6, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(201, new Skill(201, "Tir de lance-pierre", new UseSkill(BaseSkill.UseSkill), "26 -1", new string[] { "23 201" }, false, 7, 0, 0, 6, 6, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(202, new Skill(202, "Coup fort", new UseSkill(BaseSkill.UseSkill), "14 -1", new string[] { "12 202" }, false, 1, 0, 4, 4, 100, 0, 1, 1, 60, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(203, new Skill(203, "Tir d'objectif", new UseSkill(BaseSkill.UseSkill), "26 203", new string[] { "23 210" }, false, 8, 0, 2, 10, 100, 0, 2, 2, 55, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(204, new Skill(204, "Tir de torsion", new UseSkill(BaseZoneSkill.UseSkill), "14 203", new string[] { "12 204" }, false, 3, 0, 12, 10, 200, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(205, new Skill(205, "Assomer", new UseSkill(BaseSkill.UseSkill), "14 203", new string[] { "13 205" }, false, 1, 0, 7, 4, 150, 0, 1, 1, 80, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(206, new Skill(206, "Cri de combat", new UseSkill(BuffSkill.UseSkill), "22 206", new string[] { "24 207" }, false, 0, 0, 8, 14, 150, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));

            SkillsManager.skills.Add(220, new Skill(220, "Coupe de base", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 513", "11 513", "40 513", "25 255", "13 524" }, false, 1, 0, 0, 0, 8, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(221, new Skill(221, "Tir d´arbalète", new UseSkill(BaseSkill.UseSkill), "26 -1", new string[] { "23 514" }, false, 7, 0, 0, 4, 8, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, null));

            SkillsManager.skills.Add(240, new Skill(240, "Tir à l'arc", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 257", "11 257", "13 270" }, false, 8, 0, 0, 0, 8, 0, 2, 1, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(241, new Skill(241, "Coup de dague", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "23 258" }, false, 1, 0, 0, 0, 8, 0, 1, 2, 0, 0, 0, 0, 0, 0, 0, null));


            SkillsManager.skills.Add(260, new Skill(260, "Boulon d´énergie", new UseSkill(BaseSkill.UseSkill), "14 593", new string[] { "11 594" }, false, 6, 0, 5, 4, 10, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(261, new Skill(261, "Tir d´arme de charme", new UseSkill(BaseSkill.UseSkill), "26 -1", new string[] { "23 595" }, false, 7, 0, 1, 0, 8, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, null));

            //SP1Escrimeur Skills
            SkillsManager.skills.Add(811, new Skill(811, "Attaque d´épée à deux mains", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4100", "11 4100", "12 4101", "13 4102" }, false, 1, 0, 0, 0, 8, 1, 1, 1, 80, 120, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(812, new Skill(812, "Chute triple", new UseSkill(BaseSkill.UseSkill), "14 4103", new string[] { "23 4104" }, false, 1, 0, 20, 2, 60, 1, 1, 1, 700, 400, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(813, new Skill(813, "Peau de fer", new UseSkill(BuffSkill.UseSkill), "22 4105", new string[] { "24 4106" }, false, 0, 0, 120, 8, 1200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(814, new Skill(814, "Souffle de torsion", new UseSkill(BaseZoneSkill.UseSkill), "14 4107", new string[] { "25 4108" }, false, 3, 0, 58, 4, 180, 1, 1, 1, 250, 200, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(815, new Skill(815, "Provoquer", new UseSkill(BaseAttractSkill.UseSkill), "22 4109", new string[] { "24 4110" }, false, 5, 0, 80, 2, 300, 0, 0, 0, 0, 0, 0, 0, 212, 0, 0, null));
            SkillsManager.skills.Add(816, new Skill(816, "Finir de souffler", new UseSkill(SpecialCibleSkill.UseSkill), "14 4111", new string[] { "40 4112" }, false, 3, 0, 65, 4, 200, 1, 1, 1, 650, 350, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(817, new Skill(817, "Peur de cri", new UseSkill(BuffSkill.UseSkill), "22 4113", new string[] { "24 4114" }, false, 4, 0, 150, 5, 1200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(818, new Skill(818, "Fraction d´épaule", new UseSkill(BaseMoveSkill.UseSkill), "14 -1", new string[] { "41 4115" }, false, 4, 0, 100, 2, 130, 1, 1, 1, 200, 100, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(819, new Skill(819, "Purée", new UseSkill(BuffSkill.UseSkill), "22 4116", new string[] { "24 4117" }, false, 5, 0, 220, 4, 700, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(820, new Skill(820, "Choc de terre", new UseSkill(BaseZoneSkill.UseSkill), "14 -1", new string[] { "42 4118" }, false, 4, 0, 140, 5, 500, 1, 1, 1, 800, 400, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(821, new Skill(821, "Lever du dragon", new UseSkill(CibleZoneSkill.UseSkill), "14 4119", new string[] { "44 4120" }, false, 2, 2, 180, 6, 1200, 1, 1, 1, 800, 1400, 0, 0, 0, 0, 0, null));

            //SP2Escri Skills
            SkillsManager.skills.Add(822, new Skill(822, "Coupe de base", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4300", "12 4301", "13 4302" }, false, 1, 0, 0, 0, 4, 2, 1, 1, 90, 70, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(823, new Skill(823, "Couper en croix", new UseSkill(BaseSkill.UseSkill), "14 4303", new string[] { "23 4304" }, false, 1, 0, 18, 0, 50, 2, 1, 1, 300, 100, 30, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(824, new Skill(824, "Lame d'énergie", new UseSkill(SpecialCibleSkill.UseSkill), "14 -1", new string[] { "12 4305" }, false, 6, 0, 45, 4, 200, 2, 1, 1, 150, 650, 0, 30, 0, 0, 0, null));

            //SP1Archer Skills
            SkillsManager.skills.Add(833, new Skill(833, "Tir à l'arc", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4200", "11 4200", "12 4201", "13 4220" }, false, 8, 0, 0, 0, 8, 2, 2, 1, 80, 120, 0, 0, 0, 0, 0, null));
            buffs = new List<SkillBuff>();
            buffs.Add(new SkillBuff(100, 74));
            SkillsManager.skills.Add(834, new Skill(834, "Œil de faucon", new UseSkill(BuffSkill.UseSkill), "22 4202", new string[] { "24 4203" }, false, 4, 0, 150, 10, 300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, buffs));
            SkillsManager.skills.Add(835, new Skill(835, "Tir de tête", new UseSkill(BaseSkill.UseSkill), "20 4204", new string[] { "23 4205" }, false, 8, 0, 45, 6, 70, 2, 2, 1, 250, 180, 50, 0, 0, 0, 0, null));
            buffs = new List<SkillBuff>();
            buffs.Add(new SkillBuff(100, 75));
            SkillsManager.skills.Add(836, new Skill(836, "Marcheur de vent", new UseSkill(BuffSkill.UseSkill), "22 4206", new string[] { "24 4207" }, false, 4, 0, 250, 10, 600, 2, 2, 1, 0, 0, 0, 0, 0, 0, 0, buffs));
            SkillsManager.skills.Add(837, new Skill(837, "Coup de foudre", new UseSkill(CibleZoneSkill.UseSkill), "14 -1", new string[] { "25 4208" }, false, 8, 2, 145, 5, 400, 2, 2, 1, 250, 150, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(838, new Skill(838, "Triple flèche", new UseSkill(CibleZoneSkill.UseSkill), "14 -1", new string[] { "40 4209" }, false, 8, 1, 80, 2, 300, 2, 2, 1, 500, 350, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(839, new Skill(839, "Hausse de portée", new UseSkill(BaseSkill.UseSkill), "14 4210", new string[] { "23 4211" }, false, 12, 0, 120, 12, 350, 2, 2, 1, 800, 500, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(840, new Skill(840, "Boulon de vis", new UseSkill(SpecialCibleSkill.UseSkill), "14 4212", new string[] { "23 4213" }, false, 8, 0, 100, 4, 200, 2, 2, 1, 200, 1300, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(841, new Skill(841, "Feu couvrant", new UseSkill(BaseZoneSkill.UseSkill), "14 4214", new string[] { "41 4215" }, false, 4, 0, 135, 4, 600, 2, 2, 1, 800, 400, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(842, new Skill(842, "Rapide", new UseSkill(BuffSkill.UseSkill), "22 4216", new string[] { "24 4217" }, false, 0, 0, 300, 10, 1200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(843, new Skill(843, "Orage", new UseSkill(CibleZoneSkill.UseSkill), "14 4218", new string[] { "24 4219" }, false, 10, 3, 200, 8, 1200, 2, 2, 1, 1200, 800, 0, 0, 0, 0, 0, null));

            //SP2Archer Skills
            SkillsManager.skills.Add(844, new Skill(844, "Attaque de dague", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4500", "11 4500", "12 4501", "13 4502" }, false, 1, 0, 0, 0, 4, 4, 1, 2, 50, 80, 0, 0, 0, 0, 0, null));

            //SP1Mage Skills
            SkillsManager.skills.Add(855, new Skill(855, "Boule de feu", new UseSkill(BaseSkill.UseSkill), "-1 4000", new string[] { "11 4001" }, false, 7, 0, 40, 4, 8, 1, 3, 1, 100, 250, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(856, new Skill(856, "Irruption de feu", new UseSkill(CibleZoneSkill.UseSkill), "-1 4002", new string[] { "11 4003" }, false, 8, 1, 65, 6, 70, 1, 3, 1, 250, 650, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(857, new Skill(857, "Haleine de feu", new UseSkill(BaseSkill.UseSkill), "-1 4010", new string[] { "11 4011" }, false, 5, 0, 140, 10, 250, 1, 3, 1, 450, 700, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(858, new Skill(858, "Transfusion de Mana", new UseSkill(BuffSkill.UseSkill), "22 4006", new string[] { "24 4007" }, false, 0, 0, 0, 8, 1800, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(859, new Skill(859, "Tempête de feu", new UseSkill(BaseZoneSkill.UseSkill), "14 4008", new string[] { "12 4009" }, false, 2, 0, 230, 6, 400, 1, 3, 1, 300, 800, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(860, new Skill(860, "Javelot de feu", new UseSkill(BaseSkill.UseSkill), "20 4004", new string[] { "13 4005" }, false, 10, 0, 100, 5, 400, 1, 3, 1, 700, 1200, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(861, new Skill(861, "Bénédiction de feu", new UseSkill(BuffSkill.UseSkill), "22 4012", new string[] { "24 4013" }, false, 5, 0, 570, 12, 3000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(862, new Skill(862, "Mur coupe-feu", new UseSkill(SpecialCibleSkill.UseSkill), "-1 4014", new string[] { "11 4015" }, false, 6, 0, 340, 8, 450, 1, 3, 1, 400, 1500, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(863, new Skill(863, "Volée de météorite", new UseSkill(CibleZoneSkill.UseSkill), "20 4016", new string[] { "13 4017" }, false, 9, 3, 370, 8, 700, 1, 3, 1, 800, 1000, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(864, new Skill(864, "Inferno", new UseSkill(BaseZoneSkill.UseSkill), "14 4018", new string[] { "12 4019" }, false, 4, 0, 380, 10, 800, 1, 3, 1, 300, 2000, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(865, new Skill(865, "Attaque de météorite", new UseSkill(CibleZoneSkill.UseSkill), "20 4020", new string[] { "13 4021" }, false, 9, 4, 450, 12, 1800, 1, 3, 1, 1200, 1500, 0, 0, 0, 0, 0, null));

            //SP2Mage Skills
            SkillsManager.skills.Add(866, new Skill(866, "Attaque sacrée", new UseSkill(BaseSkill.UseSkill), "-1 4400", new string[] { "-1 4401" }, false, 8, 0, 0, 4, 8, 3, 3, 1, 40, 90, 0, 0, 0, 0, 0, null));

            //SP3Escrimeur Skills
            SkillsManager.skills.Add(889, new Skill(889, "Tir d'arbalète", new UseSkill(BaseSkill.UseSkill), "10 3400", new string[] { "11 3401" }, false, 7, 0, 0, 3, 8, 3, 2, 2, 150, 200, 0, 0, 0, 0, 0, null));

            //SP4Escrimeur Skills
            SkillsManager.skills.Add(900, new Skill(900, "Balance le bras de la baguette", new UseSkill(BaseSkill.UseSkill), "10 -1", new string[] { "11 3500", "11 3500", "12 3521", "13 3522" }, false, 2, 0, 0, 0, 8, 4, 1, 1, 120, 250, 0, 0, 0, 0, 0, null));

            //SP3Archer Skills 
            SkillsManager.skills.Add(911, new Skill(911, "Fusil à pompe", new UseSkill(BaseSkill.UseSkill), "10 -1", new string[] { "11 3601", "11 3601", "12 3601", "13 3602" }, false, 6, 0, 0, 2, 8, 1, 2, 1, 150, 180, 0, 0, 0, 0, 0, null));

            //SP4Archer Skills
            SkillsManager.skills.Add(922, new Skill(922, "Jet de boomerang", new UseSkill(BaseSkill.UseSkill), "10 -1", new string[] { "11 3700", "11 3700", "12 3721", "13 3722" }, false, 6, 0, 0, 1, 7, 3, 2, 1, 110, 160, 0, 0, 0, 0, 0, null));
            //SkillsManager.skills.Add(923, new Skill(923, "Attaque du serpent", 6, false, "20 3701", new string[] { "13 3702" }, 6, 3, 50, 2, 70, 3, 2, 1, 240, 360, 0, 0, 0, 0, 0));

            //SP3Mage Skills
            SkillsManager.skills.Add(933, new Skill(933, "Boule de glace", new UseSkill(BaseSkill.UseSkill), "10 3800", new string[] { "11 3801" }, false, 7, 0, 80, 4, 8, 2, 3, 1, 120, 250, 0, 0, 0, 0, 0, null));

            //SP4Mage Skills
            SkillsManager.skills.Add(944, new Skill(944, "Fait feu avec le pistolet magique", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 3900", "11 3900", "12 3901", "13 3921" }, false, 6, 0, 10, 1, 7, 4, 2, 2, 160, 250, 0, 0, 0, 0, 0, null));

            //SP5Escri Skills
            SkillsManager.skills.Add(1056, new Skill(1056, "Attaque basique", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4991", "11 4991", "12 4992" }, false, 2, 0, 0, 0, 5, 1, 1, 1, 90, 70, 0, 0, 0, 0, 0, null));

            //SP5Archer Skills
            SkillsManager.skills.Add(1067, new Skill(1067, "Coup de canon", new UseSkill(BaseSkill.UseSkill), "20 4578", new string[] { "11 4560" }, false, 11, 0, 0, 3, 9, 1, 2, 1, 100, 150, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1068, new Skill(1068, "Coup de canon", new UseSkill(BaseSkill.UseSkill), "20 4578", new string[] { "11 4560" }, false, 11, 0, 0, 3, 9, 1, 2, 1, 100, 150, 0, 0, 0, 0, 0, null));

            //SP5Mage Skills
            SkillsManager.skills.Add(1078, new Skill(1078, "Boule de magma", new UseSkill(BaseSkill.UseSkill), "20 4539", new string[] { "11 4523" }, false, 9, 0, 60, 3, 9, 1, 3, 1, 100, 180, 0, 0, 0, 0, 0, null));

            //SP6Escri Skills
            SkillsManager.skills.Add(1089, new Skill(1089, "Attaque à la lance de base", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4330", "11 4330", "12 4330" }, false, 2, 0, 0, 1, 7, 2, 1, 1, 60, 40, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1090, new Skill(1090, "Séparation de la mer", new UseSkill(SpecialCibleSkill.UseSkill), "34 -1", new string[] { "45 4341" }, false, 2, 0, 110, 1, 100, 2, 1, 1, 180, 120, 0, 0, 0, 0, 0, null));
            //SkillsManager.skills.Add(1091, new Skill(1091, "Coup précis", 5, false, "34 4351", new string[] { "13 4331" }, 13, 0, 80, 4, 120, 2, 1, 1, 330, 190, 0, 0, 0, 0, 0));
            //SkillsManager.skills.Add(1092, new Skill(1092, "Violent tourbillon", 8, false, "-1 -1", new string[] { "41 4333" }, 10, 1, 200, 0, 300, 2, 1, 1, 440, 330, 0, 0, 0, 0, 0));
            SkillsManager.skills.Add(1093, new Skill(1093, "Sept coups", new UseSkill(SpecialCibleSkill.UseSkill), "-1 -1", new string[] { "25 4337" }, false, 3, 0, 90, 0, 120, 2, 1, 1, 120, 80, 0, 0, 0, 0, 0, null));
            //SkillsManager.skills.Add(1094, new Skill(1094, "Cyclone", 9, true, "-1 -1", new string[] { "42 4334" }, 4, 0, 280, 0, 250, 2, 1, 1, 550, 300, 0, 0, 212, 0, 0));
            SkillsManager.skills.Add(1095, new Skill(1095, "Sacrifice", new UseSkill(BuffSkill.UseSkill), "-1 -1", new string[] { "21 4340" }, false, 10, 0, 400, 1, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1096, new Skill(1096, "Méditation", new UseSkill(BuffSkill.UseSkill), "-1 -1", new string[] { "20 4339" }, false, 0, 0, 220, 1, 400, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1097, new Skill(1097, "Lance spiraliforme", new UseSkill(SpecialCibleSkill.UseSkill), "-1 -1", new string[] { "23 4332" }, false, 10, 0, 150, 0, 500, 2, 1, 1, 330, 230, 15, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1098, new Skill(1098, "Paroles de Bouddha", new UseSkill(BuffSkill.UseSkill), "-1 -1", new string[] { "21 4338" }, false, 6, 0, 900, 1, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1099, new Skill(1099, "Nouveau départ", new UseSkill(BaseZoneSkill.UseSkill), "21 4353", new string[] { "24 4346" }, false, 5, 0, 600, 3, 1000, 2, 1, 1, 1350, 1150, 0, 0, 0, 0, 0, null));
            //SkillsManager.skills.Add(1100, new Skill(1100, "Harpon dragonesque", 4, false, "-1 -1", new string[] { "44 4349" }, 7, 0, 250, 2, 200, 2, 1, 1, 0, 0, 0, 0, 0, 0, 0));

            //SP6Archer Skills
            SkillsManager.skills.Add(1114, new Skill(1114, "Tir d'arbalète", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 4260" }, false, 8, 0, 0, 1, 4, 2, 2, 1, 100, 100, 0, 0, 0, 0, 0, null));
            SkillsManager.skills.Add(1115, new Skill(1115, "Faucon en chute libre", new UseSkill(BaseSkill.UseSkill), "14 4263", new string[] { "12 4264" }, false, 10, 0, 150, 4, 70, 2, 2, 1, 200, 300, 10, 0, 0, 0, 0, null));
            //SkillsManager.skills.Add(1116, new Skill(1116, "Piège mobile", 3, true, "-1 -1", new string[] { "13 4277" }, 0, 0, 120, 0, 120, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
            //SkillsManager.skills.Add(1117, new Skill(1117, "Épier", 3, false, "14 4279", new string[] { "12 -1" }, 13, 0, 200, 4, 300, 0, 0, 0, 0, 0, 0, 0, 0, 4269, 60));

            //SP6Mage Skills
            SkillsManager.skills.Add(1102, new Skill(1102, "Triton fulgurant", new UseSkill(BaseSkill.UseSkill), "31 4374", new string[] { "11 4360" }, false, 10, 0, 50, 3, 9, 2, 3, 1, 100, 150, 0, 0, 0, 0, 0, null));

            //SP8Escri Skills
            SkillsManager.skills.Add(1338, new Skill(1338, "Lumière et Obscurité", new UseSkill(BaseSkill.UseSkill), "-1 -1", new string[] { "11 3850", "11 3850", "12 3851" }, false, 1, 0, 0, 0, 3, 3, 1, 1, 0, 0, 0, 0, 0, 0, 0, null));
            /*SkillsManager.skills.Add(1339, new Skill(1339, "Jugement de lumière", 4, false, "14 3852", new string[] { "13 3853" }, 4, 0, 60, 4, 60, 3, 1, 1, 0, 0, 0, 0, 0, 0, 0));
            SkillsManager.skills.Add(1340, new Skill(1340, "Jugement de damnation", 4, false, "21 3854", new string[] { "40 3855" }, 3, 0, 180, 10, 120, 4, 2, 2, 0, 0, 0, 0, 0, 0, 0));
            SkillsManager.skills.Add(1341, new Skill(1341, "Justice", 6, false, "24 3857", new string[] { "34 3857" }, 10, 2, 90, 0, 160, 3, 1, 1, 0, 0, 0, 0, 0, 0, 0));*/

            Dictionary<int, int> AdventerSkills = new Dictionary<int, int>();
            Dictionary<int, int> SwordmanSkills = new Dictionary<int, int>();
            Dictionary<int, int> ArcherSkills = new Dictionary<int, int>();
            Dictionary<int, int> MagicianSkills = new Dictionary<int, int>();

            AdventerSkills.Add(0, 200);
            AdventerSkills.Add(1, 201);
            AdventerSkills.Add(2, 202);
            AdventerSkills.Add(3, 203);
            AdventerSkills.Add(4, 204);
            AdventerSkills.Add(5, 205);
            AdventerSkills.Add(6, 206);

            SwordmanSkills.Add(0, 220);
            SwordmanSkills.Add(1, 221);

            ArcherSkills.Add(0, 240);
            ArcherSkills.Add(1, 241);

            MagicianSkills.Add(0, 260);
            MagicianSkills.Add(1, 261);

            classSkills.Add(0, AdventerSkills);
            classSkills.Add(1, SwordmanSkills);
            classSkills.Add(2, ArcherSkills);
            classSkills.Add(3, MagicianSkills);

            //SP1Escrimeur Skills
            Dictionary<int, int> spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 811);
            spSkills.Add(1, 812);
            spSkills.Add(2, 813);
            spSkills.Add(3, 814);
            spSkills.Add(4, 815);
            spSkills.Add(5, 816);
            spSkills.Add(6, 817);
            spSkills.Add(7, 818);
            spSkills.Add(8, 819);
            spSkills.Add(9, 820);
            spSkills.Add(10, 821);

            spsSkills.Add(2, spSkills);

            //SP2Escri Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 822);
            spSkills.Add(1, 823);
            spSkills.Add(2, 824);

            spsSkills.Add(3, spSkills);

            //SP1Archer Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 833);
            spSkills.Add(1, 834);
            spSkills.Add(2, 835);
            spSkills.Add(3, 836);
            spSkills.Add(4, 837);
            spSkills.Add(5, 838);
            spSkills.Add(6, 839);
            spSkills.Add(7, 840);
            spSkills.Add(8, 841);
            spSkills.Add(9, 842);
            spSkills.Add(10, 843);

            spsSkills.Add(4, spSkills);

            //SP2Archer Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 844);

            spsSkills.Add(5, spSkills);

            //SP1Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 855);
            spSkills.Add(1, 856);
            spSkills.Add(2, 857);
            spSkills.Add(3, 858);
            spSkills.Add(4, 859);
            spSkills.Add(5, 860);
            spSkills.Add(6, 861);
            spSkills.Add(7, 862);
            spSkills.Add(8, 863);
            spSkills.Add(9, 864);
            spSkills.Add(10, 865);

            spsSkills.Add(6, spSkills);

            //SP2Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 866);

            spsSkills.Add(7, spSkills);

            //SP3Escrimeur Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 889);

            spsSkills.Add(10, spSkills);

            //SP4Escrimeur Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 900);

            spsSkills.Add(11, spSkills);

            //SP3Archer Skills 
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 911);

            spsSkills.Add(12, spSkills);

            //SP4Archer Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 922);
            //spSkills.Add(1, 923);

            spsSkills.Add(13, spSkills);

            //SP3Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 933);

            spsSkills.Add(14, spSkills);

            //SP4Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 944);

            spsSkills.Add(15, spSkills);

            //SP5Escri Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1056);

            spsSkills.Add(17, spSkills);

            //SP5Archer Skills
            /*spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1067);
            spSkills.Add(1, 1068);
            spSkills.Add(2, 1069);
            spSkills.Add(3, 1070);
            spSkills.Add(4, 1071);
            spSkills.Add(5, 1072);
            spSkills.Add(6, 1073);
            spSkills.Add(7, 1074);
            spSkills.Add(8, 1075);
            spSkills.Add(9, 1076);
            spSkills.Add(10, 1077);*/


            spsSkills.Add(18, spSkills);

            //SP5Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1078);

            spsSkills.Add(19, spSkills);

            //SP6Escri Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1089);
            /*spSkills.Add(1, 1090);
            spSkills.Add(2, 1091);
            spSkills.Add(3, 1092);
            spSkills.Add(4, 1093);
            spSkills.Add(5, 1094);
            spSkills.Add(6, 1095);
            spSkills.Add(7, 1096);
            spSkills.Add(8, 1097);
            spSkills.Add(9, 1098);
            spSkills.Add(10, 1099);*/

            spsSkills.Add(20, spSkills);

            //SP6Archer Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1114);
            /* spSkills.Add(1, 1115);
             spSkills.Add(2, 1116);
             spSkills.Add(3, 1117);*/

            spsSkills.Add(21, spSkills);

            //SP6Mage Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1102);

            spsSkills.Add(22, spSkills);

            //SP8Escri Skills
            spSkills = new Dictionary<int, int>();
            spSkills.Add(0, 1338);
            /* spSkills.Add(1, 1339);
             spSkills.Add(2, 1340);
             spSkills.Add(3, 1341);*/

            spsSkills.Add(26, spSkills);
        }
        public static double GetBonusElement(int element_base, int element_cible)
        {
            double bonus = 0.0;
            if (element_base == 1)
            {
                switch (element_cible)
                {
                    case 0:
                        bonus = 30.0;
                        break;
                    case 2:
                        bonus = 100.0;
                        break;
                    case 4:
                        bonus = 50.0;
                        break;
                }
            }
            if (element_base == 2)
            {
                switch (element_cible)
                {
                    case 0:
                        bonus = 30.0;
                        break;
                    case 1:
                        bonus = 100.0;
                        break;
                    case 3:
                        bonus = 50.0;
                        break;
                }
            }
            if (element_base == 3)
            {
                switch (element_cible)
                {
                    case 0:
                        bonus = 30.0;
                        break;
                    case 1:
                        bonus = 50.0;
                        break;
                    case 4:
                        bonus = 200.0;
                        break;
                }
            }
            if (element_base == 4)
            {
                switch (element_cible)
                {
                    case 0:
                        bonus = 30.0;
                        break;
                    case 2:
                        bonus = 50.0;
                        break;
                    case 3:
                        bonus = 200.0;
                        break;
                }
            }
            return bonus;
        }

        public delegate void UseSkill(Map map, Entitie sender, Entitie cible, EntitieSkill skill, int x, int y);
    }
}
