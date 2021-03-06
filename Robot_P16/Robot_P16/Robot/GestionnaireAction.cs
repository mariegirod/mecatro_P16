using System;
using System.Collections;
using Microsoft.SPOT;
using Robot_P16.Actions;
using Robot_P16.Robot.composants.Servomoteurs;

using Robot_P16.Map;
namespace Robot_P16.Robot
{
    public class GestionnaireAction
    {

        private static Hashtable ACTION_PER_TYPE = new Hashtable();

        private static readonly int PERIOD_SEND_POSITION = 1000; // En ms
        private static readonly Action ACTION_SEND_POSITION =
            new ActionBuilder("Action send Position mère").Add(
                    new ActionBuilder("Action send position - Action get position").BuildActionGetPosition()
                )
                .Add(
                    new ActionBuilder("Action send position - Période du signal").BuildActionWait(PERIOD_SEND_POSITION)
                ).BuildActionEnSerieRepeated(-1); // Envois infinis

        public static void loadActions()
        {
            ACTION_PER_TYPE.Clear();
            //loadActionHomologation();
            loadActionTest1();
            //loadActionPRCompete();

            //loadActionPRServos();

            //loadActionTestGR();
            Informations.printInformations(Priority.HIGH, "actions chargees");
        }

        public static void startActions(ModeOperatoire mode, TypeRobot type)
        {
            Debug.Print("Starting actions with mod : " + mode.ToString() + " & type : " + type.ToString());
            if (ACTION_PER_TYPE.Contains(mode))
            {
                if (((Hashtable)ACTION_PER_TYPE[mode]).Contains(type))
                {
                    Debug.Print("Lancement de l'action m�re avec le mode " + mode.ToString() + " & type : " + type.ToString());
                    ((Action)((Hashtable)ACTION_PER_TYPE[mode])[type]).Execute();
                    return;
                }
                else
                {
                    Debug.Print("MODE INTROUVABLE : Impossible de lancer l'action m�re (introuvable) pour le mode : " + mode.ToString());
                }
            }
            else
            {
                Debug.Print("TYPE INTROUVABLE : Impossible de lancer l'action m�re (introuvable) pour le type : " + type.ToString());
            }
        }

        private static void loadActionTestGR()
        {
            Action MOTHER_ACTION = new ActionBuilder("Action test GR").Add(
                new ActionBuilder("Lanceur test 1").BuildActionDelegate(() => Robot.robot.GR_LANCEUR_BALLE.launchSpeed(0.5d))
                ).Add(
                new ActionBuilder("Wait a bit").BuildActionWait(3000)
                ).Add(
                new ActionBuilder("Lanceur test 1").BuildActionDelegate(() => Robot.robot.GR_LANCEUR_BALLE.stop())
                ).BuildActionEnSerie();
            setMotherAction(ModeOperatoire.COMPETITION, TypeRobot.GRAND_ROBOT, MOTHER_ACTION);

            /*Action TEST_ACTION = new ActionJack();
            setMotherAction(ModeOperatoire.COMPETITION, TypeRobot.PETIT_ROBOT, TEST_ACTION);*/
        }

        private static void loadActionHomologation()
        {
            // Blablabla
            Action MOTHER_ACTION = StrategiePetitRobot.loadActionsPetitRobot();

            setMotherAction(ModeOperatoire.HOMOLOGATION, TypeRobot.PETIT_ROBOT, MOTHER_ACTION);
        }

        private static void loadActionPRCompete()
        {
            PointOriente pt1 = new PointOriente(0, 100, 50);
            PointOriente pt2 = new PointOriente(0, 1000, 50);
            PointOriente pt3 = new PointOriente(100, 0, 50);
            PointOriente pt4 = new PointOriente(0, 0, 0);

            /*Action MOTHER_ACTION = new ActionBuilder("Action m�re Test1").Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(5000)
                ).Add(
                    new Actions.ActionBaseRoulante("Point1 ")
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(5000)
                )
                .Add(
                   new Actions.ActionBaseRoulante("Point2 ", pt2)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(5000)
                )
                .Add(
                   new Actions.ActionBaseRoulante("Point3 ", pt3)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(5000)
                )
                .Add(
                   new Actions.ActionBaseRoulante("Point4 ", pt4)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(5000)
                )
                .BuildActionEnSerie(); // Envois infinis

            setMotherAction(ModeOperatoire.TEST1,TypeRobot.PETIT_ROBOT, MOTHER_ACTION);
            setMotherAction(ModeOperatoire.TEST1, TypeRobot.GRAND_ROBOT, MOTHER_ACTION);*/
        }


        private static void loadActionPRServos()
        {

            Action MOTHER_ACTION = new ActionBuilder("Action m�re Test1")
                    .Add(new ActionBuilder("Action servo test").BuildActionVentouze(VENTOUZES.VENTOUZE_DROITE, true))
                     .Add(new ActionWait("wait a bit", 5000))
                     .Add(new ActionBuilder("Action servo test").BuildActionVentouze(VENTOUZES.VENTOUZE_DROITE, false))
                     .Add(new ActionWait("wait a bit", 2000))
                     .Add(new ActionBuilder("Action servo test").BuildActionVentouze(VENTOUZES.VENTOUZE_GAUCHE, true))
                     .Add(new ActionWait("wait a bit", 2000))
                     .Add(new ActionBuilder("Action servo test").BuildActionVentouze(VENTOUZES.VENTOUZE_GAUCHE, false))
                   /*.Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 819)
                   )
                   .Add(new ActionWait("wait a bit", 5000))
                   .Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse gauche").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 477)
                   )*//*.Add(
                     new ActionBuilder("Action servo test").BuildActionVentouze(VENTOUZES.VENTOUZE_GAUCHE, false)
                   ).Add(
                    new ActionBuilder("Action servo descendre bras").BuildActionServoRotation(Robot.robot.PR_SERVO_ASCENSEUR_BRAS_GAUCHE, speed.reverse, 1000)
                   )*//*.Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)    
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 0*1024/300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 20 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 40 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 60 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 80 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 100 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 120 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 140 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 160 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 180 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 200 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 220 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 240 * 1024 / 300)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(1000)
                   ).Add(
                    new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 260 * 1024 / 300)
                   )*/
                   /*.Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(5000)
                   ).Add(
                     new ActionBuilder("Action servo test").BuildActionServo(Robot.robot.PR_SERVO_ASCENSEUR_BRAS_DROIT,
                     Robot_P16.Robot.composants.Servomoteurs.ServoCommandTypes.ABSOLUTE_ROTATION, 50)
                   ).Add(
                    new ActionBuilder("Wait a bit").BuildActionWait(5000)
                   ).Add(
                     new ActionBuilder("Action servo test").BuildActionServo(Robot.robot.PR_SERVO_ASCENSEUR_BRAS_DROIT,
                     Robot_P16.Robot.composants.Servomoteurs.ServoCommandTypes.ABSOLUTE_ROTATION, -50)
                   )new ActionBuilder("ServoPR- aiguiller la pompe sur la ventouse droite").BuildActionServoAbsolue(Robot.robot.PR_SERVO_AIGUILLAGE, 
                     */
                .BuildActionEnSerie();

            setMotherAction(ModeOperatoire.COMPETITION, TypeRobot.PETIT_ROBOT, MOTHER_ACTION);
        }

        private static void loadActionTest1()
        {
            /*Action MOTHER_ACTION = new ActionBuilder("Action mère Test1").Add(
                    new Actions.ActionsIHM.ActionBouton(Robot.robot.TR1_BOUTON_1)
                )
                .Add(
                    new ActionBuilder("Allumer LED").BuildActionDelegate(Robot.robot.TR1_BOUTON_1.TurnLedOn)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(2000)
                )
                .Add(
                    new ActionBuilder("Eteindre LED").BuildActionDelegate(Robot.robot.TR1_BOUTON_1.TurnLedOff)
                ).BuildActionEnSerieRepeated(-1); // Envois infinis

            setMotherAction(ModeOperatoire.TEST1, MOTHER_ACTION);*/

            PointOriente pt1 = new PointOriente(0, 500, 50);
            PointOriente pt2 = new PointOriente(250, 250, 50);
            PointOriente pt3 = new PointOriente(0, 0, 50);
            Action MOTHER_ACTION = new ActionBuilder("Action m�re Test1").Add(
                    new ActionBuilder("Pt1").BuildActionBaseRoulante_GOTO_ONLY(pt1)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(2000)
                )
                .Add(
                   new ActionBuilder("Pt2").BuildActionBaseRoulante_GOTO_ONLY(pt2)
                ).Add(
                    new ActionBuilder("Wait a bit...").BuildActionWait(2000)
                )
                .Add(
                   new ActionBuilder("Pt3").BuildActionBaseRoulante_GOTO_ONLY(pt3)
                )
                .BuildActionEnSerieRepeated(-1); // Envois infinis

            setMotherAction(ModeOperatoire.TEST1, TypeRobot.PETIT_ROBOT, MOTHER_ACTION);
        }

        private static void setMotherAction(ModeOperatoire mode, TypeRobot type, Action a)
        {
            if (!ACTION_PER_TYPE.Contains(mode))
            {
                ACTION_PER_TYPE.Add(mode, new Hashtable());
            }
            
            if (((Hashtable)ACTION_PER_TYPE[mode]).Contains(type))
            {
                ((Hashtable)ACTION_PER_TYPE[mode])[type] = a;
            }
            else
            {
                ((Hashtable)ACTION_PER_TYPE[mode]).Add(type, a);
            }
            Informations.printInformations(Priority.MEDIUM, "actions mere set pour le type "+type+" & le mode "+mode);
        }


    }
}
