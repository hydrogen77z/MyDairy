using System;
using System.Collections.Generic;
using System.Text;
using MyDairy.Models;
using MyDairy.Settings;

namespace MyDairy.Helpers;

public static class QuickNoteHelper
{
    public static void AddNote(string note)
    {
        if (string.IsNullOrEmpty(note))
        {
            return;
        }
        AppSettings.Instance.Notes.Add(new(note, DateTime.Now));
    }

    public static void EditNote(Note note, string newContent)
    {
        if (string.IsNullOrEmpty(newContent))
        {
            return;
        }

        note.Content = newContent;
        note.LastEditTime = DateTime.Now;
    }

    public static void RemoveNote(Note note)
    {
        AppSettings.Instance.Notes.Remove(note);
    }
}
