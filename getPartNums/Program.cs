using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms; // Add reference to System.Windows.Form

class Program
{
    [STAThread] // Important: Required for using Windows Forms dialogs
    static void Main(string[] args)
    {
        // Console.WriteLine("Enter the folder path:");
        string folderPath = OpenFolderDialog();
        // string folderPath = Console.ReadLine()?.Trim('"'); // Trim extra quotes if copied from somewhere. //Directory.GetCurrentDirectory();
        // folderPath = folderPath.EndsWith("\\") ? folderPath.Substring(0, folderPath.Length - 1) : folderPath;

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("The specified folder does not exist. Please check the path.");
            return;
        }

        // Console.WriteLine("Enter the output CSV file path (e.g., C:\\output.csv):");
        string csvPath = folderPath + "\\output_numbers.csv";    //Console.ReadLine()?.Trim('"');

        if (string.IsNullOrEmpty(csvPath))
        {
            Console.WriteLine("Invalid CSV path.");
            return;
        }

        Console.WriteLine($"Scanning folder: {folderPath}\n");
        List<(string FileName, string Number)> extractedData = ExtractTenDigitNumbers(folderPath);

        if (extractedData.Count > 0)
        {
            SaveToCsv(extractedData, csvPath);
            Console.WriteLine($"Data successfully exported to {csvPath}");
        }
        else
        {
            Console.WriteLine("No 10-digit numbers found to export.");
        }
    }

    static List<(string FileName, string Number)> ExtractTenDigitNumbers(string folderPath)
    {
        var extractedData = new List<(string FileName, string Number)>();

        try
        {
            // Get all files in the folder and subfolders
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

            // Regular expression to match exactly 10 consecutive digits
            Regex regex = new Regex(@"\d{10}");

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Console.WriteLine($"Scanning file: {fileName}");

                MatchCollection matches = regex.Matches(fileName);

                foreach (Match match in matches)
                {
                    Console.WriteLine($"Found 10-digit number: {match.Value} in file: {fileName}");
                    extractedData.Add((fileName, match.Value));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        return extractedData;
    }

    static void SaveToCsv(List<(string FileName, string Number)> data, string csvPath)
    {
        try
        {
            using (var writer = new StreamWriter(csvPath))
            {
                foreach (var entry in data)
                {
                    writer.WriteLine($"{entry.Number}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save CSV: {ex.Message}");
        }
    }

    static string OpenFolderDialog()
    {
        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        {
            folderDialog.Description = "Select a folder";
            folderDialog.ShowNewFolderButton = true;

            // Show the dialog and get the result
            DialogResult result = folderDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
            {
                return folderDialog.SelectedPath;
            }
        }
        return null;
    }
}
