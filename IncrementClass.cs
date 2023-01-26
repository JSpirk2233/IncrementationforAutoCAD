using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Exception = Autodesk.AutoCAD.Runtime.Exception;


//[assembly: CommandClass(TypeOfCoordinates(IncrementationProgram.IncrementClass))]

//This program will allow the user to select several Mtext/Text objects in AutoCAD and increment by a set number
namespace IncrementationProgram
{
    public class IncrementClass
    {
        int amountToIncrement;

        [CommandMethod("IncrementUp", CommandFlags.UsePickSet)]
        public void IncrementUp()
        {

            //Next Step is to actually turn this into 6 commands and remove the form for now. When all 6 commands work, we can put it all together in 
            //one larger program. 


            //Get the Current Document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;


            
            //Create the selection filter for weeding out everything that's not MText and Text and Mleaders
            TypedValue[] acTypeValue = new TypedValue[5];
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 1);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "TEXT"), 2);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "MULTILEADER"), 3);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 4);
            SelectionFilter sFilter = new SelectionFilter(acTypeValue);

            SelectionSet acSSet;

            //Check if there is anything in the selection set already. If there is, we skip the selection function.
            PromptSelectionResult acSSPrompt = docEd.SelectImplied();

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                acSSet = acSSPrompt.Value;
            }

            else
            {
                acSSet = docEd.GetSelection(sFilter).Value;
            }

            //Put all the selected objects into a final list
            ObjectId[] selectionSetFinal = acSSet.GetObjectIds();

            //Create 3 object ids separated by the type of object
            List<ObjectId> MTextArray = new List<ObjectId> { };
            List<ObjectId> DbText = new List<ObjectId> { };
            List<ObjectId> MLeaderText = new List<ObjectId> { };

            //process the list, and send it to one of 6 functions to count up, or count down, depending on MText
            try
            {
                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    //Separate all selections into Text, MText, and MLeader
                    foreach (ObjectId ObjectsId in selectionSetFinal)
                    {
                        var currentObjectId = _transaction.GetObject(ObjectsId, OpenMode.ForWrite);
                        {
                            if (currentObjectId is MText)
                            {
                                MTextArray.Add(currentObjectId.ObjectId);
                            }
                            else if(currentObjectId is MLeader) 
                            {
                                MLeaderText.Add(currentObjectId.ObjectId);
                            }
                            else
                            {
                                DbText.Add(currentObjectId.ObjectId);

                            }

                        }
                    }

                    _transaction.Commit();
                }
                //Ask the user for a number to increment by
                PromptIntegerOptions prIntOps = new PromptIntegerOptions("Please enter an integer to add to existing number: ");
                    prIntOps.AllowZero = true;
                    prIntOps.AllowNegative = false;
                    prIntOps.AllowNone = true;
                    prIntOps.DefaultValue = 1;
                PromptIntegerResult pResult = docEd.GetInteger(prIntOps);

                if(pResult.Status == PromptStatus.OK)
                {
                    amountToIncrement = pResult.Value;
                }


                if (MTextArray.Count > 0) 
                {
                    countUpMText(MTextArray, amountToIncrement);
                }
                if (MLeaderText.Count > 0)
                {
                    countUpMLeader(MLeaderText, amountToIncrement);

                }
                if(DbText.Count > 0)
                {

                    countUpDbText(DbText, amountToIncrement);
                    
                }
            }
            catch { }
            
            

        }

        //This program counts the MText up by the amount the user inputs
        public void countUpMText(List<ObjectId> MTextArray, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId MTextObjID in MTextArray)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_MTextObj = _transaction.GetObject(MTextObjID, OpenMode.ForWrite) as MText;
   
                
                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_MTextObj.Text, @"\d+$").Value;

                        if (current_MTextObj.Text.Length > numberToChange.Length)
                        {
                            string StringToKeep = current_MTextObj.Text;
                            StringToKeep = StringToKeep.Remove(StringToKeep.Length - numberToChange.Length);

                            //if the number is selected and nothing happens, the number is too large, change this to int32
                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            //Set the current mtext object to reflect the new number
                            current_MTextObj.Contents = StringToKeep + newNumber.ToString();
                        }
                        else
                        {
                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            //Set the current mtext object to reflect the new number
                            current_MTextObj.Contents = newNumber.ToString();
                        }
                    }

                    _transaction.Commit();
                }
            }
            catch(Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }

        }

        //This program counts the MLeader text up by the amount the user inputs
        public void countUpMLeader(List<ObjectId> MLeaderArray, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try
            {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId MLeaderObjID in MLeaderArray)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_MLeaderObj = _transaction.GetObject(MLeaderObjID, OpenMode.ForWrite) as MLeader;


                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_MLeaderObj.MText.Contents, @"\d+$").Value;

                        if(current_MLeaderObj.MText.Contents.Length > numberToChange.Length)
                        {
                            string StringToKeep = current_MLeaderObj.MText.Contents;
                            StringToKeep = StringToKeep.Remove(current_MLeaderObj.MText.Contents.Length - numberToChange.Length);

                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            //Create a new piece of MText replace the old mtext with it
                            MText replacement = new MText();
                            replacement.Contents = StringToKeep + newNumber.ToString();
                            replacement.SetPropertiesFrom(current_MLeaderObj.MText);

                            

                            //Set the current mtext object to reflect the new number
                            current_MLeaderObj.UpgradeOpen();
                            current_MLeaderObj.MText = replacement;
                        }
                        else
                        {
                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            //Create a new piece of MText replace the old mtext with it
                            MText replacement = new MText();
                            replacement.Contents = newNumber.ToString();
                            replacement.SetPropertiesFrom(current_MLeaderObj.MText);

                            //Set the current mtext object to reflect the new number
                            current_MLeaderObj.UpgradeOpen();
                            current_MLeaderObj.MText = replacement;

                        }
                        
                        

                        
                    }

                    _transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }


        }

        //This program counts the DbText text up by the amount the user inputs
        public void countUpDbText(List<ObjectId> DbText, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try
            {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId DbTextObjID in DbText)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_DbTextObj = _transaction.GetObject(DbTextObjID, OpenMode.ForWrite) as DBText;
                        

                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_DbTextObj.TextString, @"\d+$").Value;

                        if (current_DbTextObj.TextString.Length > numberToChange.Length)
                        {
                            string stringToKeep = current_DbTextObj.TextString;
                            stringToKeep = stringToKeep.Remove(current_DbTextObj.TextString.Length - numberToChange.Length);

                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            DBText newText = new DBText();
                            newText.TextString = stringToKeep + newNumber.ToString();

                            //Set the current mtext object to reflect the new number
                            current_DbTextObj.TextString = newText.TextString;
                        }
                        else
                        {
                            int newNumber = Int16.Parse(numberToChange) + amountToIncrement;

                            DBText newText = new DBText();
                            newText.TextString = newNumber.ToString();

                            //Set the current mtext object to reflect the new number
                            current_DbTextObj.TextString = newText.TextString;
                        }
                    }

                    _transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }
            //foreach(Text)

        }

        [CommandMethod("IncrementDown", CommandFlags.UsePickSet)]
        public void IncrementDown()
        {

            //Next Step is to actually turn this into 6 commands and remove the form for now. When all 6 commands work, we can put it all together in 
            //one larger program. 


            //Get the Current Document and database, and start a transaction
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;



            //Create the selection filter for weeding out everything that's not MText and Text and Mleaders
            TypedValue[] acTypeValue = new TypedValue[5];
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Operator, "<OR"), 0);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 1);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "TEXT"), 2);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Start, "MULTILEADER"), 3);
            acTypeValue.SetValue(new TypedValue((int)DxfCode.Operator, "OR>"), 4);
            SelectionFilter sFilter = new SelectionFilter(acTypeValue);

            SelectionSet acSSet;

            //Check if there is anything in the selection set already. If there is, we skip the selection function.
            PromptSelectionResult acSSPrompt = docEd.SelectImplied();

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                acSSet = acSSPrompt.Value;
            }

            else
            {
                acSSet = docEd.GetSelection(sFilter).Value;
            }

            //Put all the selected objects into a final list
            ObjectId[] selectionSetFinal = acSSet.GetObjectIds();

            //Create 3 object ids separated by the type of object
            List<ObjectId> MTextArray = new List<ObjectId> { };
            List<ObjectId> DbText = new List<ObjectId> { };
            List<ObjectId> MLeaderText = new List<ObjectId> { };

            //process the list, and send it to one of 6 functions to count up, or count down, depending on MText
            try
            {
                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    //Separate all selections into Text, MText, and MLeader
                    foreach (ObjectId ObjectsId in selectionSetFinal)
                    {
                        var currentObjectId = _transaction.GetObject(ObjectsId, OpenMode.ForWrite);
                        {
                            if (currentObjectId is MText)
                            {
                                MTextArray.Add(currentObjectId.ObjectId);
                            }
                            else if (currentObjectId is MLeader)
                            {
                                MLeaderText.Add(currentObjectId.ObjectId);
                            }
                            else
                            {
                                DbText.Add(currentObjectId.ObjectId);

                            }

                        }
                    }

                    _transaction.Commit();
                }
                //Ask the user for a number to increment by
                PromptIntegerOptions prIntOps = new PromptIntegerOptions("Please enter an integer to subtract from the existing number: ");
                prIntOps.AllowZero = true;
                prIntOps.AllowNegative = false;
                prIntOps.AllowNone = true;
                prIntOps.DefaultValue = 1;
                PromptIntegerResult pResult = docEd.GetInteger(prIntOps);

                if (pResult.Status == PromptStatus.OK)
                {
                    amountToIncrement = pResult.Value;
                }


                if (MTextArray.Count > 0)
                {
                    countDownMText(MTextArray, amountToIncrement);
                }
                if (MLeaderText.Count > 0)
                {
                    countDownMLeader(MLeaderText, amountToIncrement);

                }
                if (DbText.Count > 0)
                {

                    countDownDbText(DbText, amountToIncrement);

                }
            }
            catch { }



        }

        //This program counts the MText down by the amount the user inputs
        public void countDownMText(List<ObjectId> MTextArray, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try
            {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId MTextObjID in MTextArray)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_MTextObj = _transaction.GetObject(MTextObjID, OpenMode.ForWrite) as MText;


                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_MTextObj.Text, @"\d+$").Value;

                        if (current_MTextObj.Text.Length > numberToChange.Length)
                        {
                            string StringToKeep = current_MTextObj.Text;
                            StringToKeep = StringToKeep.Remove(StringToKeep.Length - numberToChange.Length);

                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }

                            //Set the current mtext object to reflect the new number
                            current_MTextObj.Contents = StringToKeep + newNumber.ToString();
                        }
                        else
                        {
                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }

                            //Set the current mtext object to reflect the new number
                            current_MTextObj.Contents = newNumber.ToString();
                        }
                    }

                    _transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }

        }

        //This program counts the MLeader text down by the amount the user inputs
        public void countDownMLeader(List<ObjectId> MLeaderArray, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try
            {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId MLeaderObjID in MLeaderArray)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_MLeaderObj = _transaction.GetObject(MLeaderObjID, OpenMode.ForWrite) as MLeader;


                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_MLeaderObj.MText.Contents, @"\d+$").Value;

                        if (current_MLeaderObj.MText.Contents.Length > numberToChange.Length)
                        {
                            string StringToKeep = current_MLeaderObj.MText.Contents;
                            StringToKeep = StringToKeep.Remove(current_MLeaderObj.MText.Contents.Length - numberToChange.Length);

                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }
                            

                            //Create a new piece of MText replace the old mtext with it
                            MText replacement = new MText();
                            replacement.Contents = StringToKeep + newNumber.ToString();
                            replacement.SetPropertiesFrom(current_MLeaderObj.MText);



                            //Set the current mtext object to reflect the new number
                            current_MLeaderObj.UpgradeOpen();
                            current_MLeaderObj.MText = replacement;
                        }
                        else
                        {
                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }

                            //Create a new piece of MText replace the old mtext with it
                            MText replacement = new MText();
                            replacement.Contents = newNumber.ToString();
                            replacement.SetPropertiesFrom(current_MLeaderObj.MText);

                            //Set the current mtext object to reflect the new number
                            current_MLeaderObj.UpgradeOpen();
                            current_MLeaderObj.MText = replacement;

                        }




                    }

                    _transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }


        }

        //This program counts the DbText text down by the amount the user inputs
        public void countDownDbText(List<ObjectId> DbText, int amountToIncrement)
        {
            //Standard AutoCAD Stuff
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            Editor docEd = acDoc.Editor;

            try
            {

                using (Transaction _transaction = acDoc.Database.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId DbTextObjID in DbText)
                    {
                        //get the last few numbers of each piece of text/mtext/multileader within array, create a number 
                        //from the last part of the text, hold the rest of the text in a placeholder. Add numberInBox to the number
                        //taken from the text. Reassemble the entire text, save the entity. 

                        var current_DbTextObj = _transaction.GetObject(DbTextObjID, OpenMode.ForWrite) as DBText;


                        //this section of code handles the numbers and does the adding
                        //int incrementationAmount = form1.numberInBox;
                        var numberToChange = Regex.Match(current_DbTextObj.TextString, @"\d+$").Value;

                        if (current_DbTextObj.TextString.Length > numberToChange.Length)
                        {
                            string stringToKeep = current_DbTextObj.TextString;
                            stringToKeep = stringToKeep.Remove(current_DbTextObj.TextString.Length - numberToChange.Length);

                            //add a check to determine if the number is lower than 0
                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }

                            DBText newText = new DBText();
                            newText.TextString = stringToKeep + newNumber.ToString();

                            //Set the current mtext object to reflect the new number
                            current_DbTextObj.TextString = newText.TextString;
                        }
                        else
                        {
                            int newNumber;
                            if (Int16.Parse(numberToChange) - amountToIncrement < 0)
                            {
                                newNumber = 0;
                            }
                            else
                            {
                                newNumber = Int16.Parse(numberToChange) - amountToIncrement;
                            }

                            DBText newText = new DBText();
                            newText.TextString = newNumber.ToString();

                            //Set the current mtext object to reflect the new number
                            current_DbTextObj.TextString = newText.TextString;
                        }
                    }

                    _transaction.Commit();
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Application.ShowAlertDialog(ex.Message);
            }
            //foreach(Text)

        }
    }
}
