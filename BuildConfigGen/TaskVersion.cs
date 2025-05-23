﻿using System;
using System.Globalization;

internal class TaskVersion : IComparable<TaskVersion>, IEquatable<TaskVersion>
{
    public TaskVersion(String version)
    {
        Int32 major, minor, patch;
        String? semanticVersion;

        VersionParser.ParseVersion(version, out major, out minor, out patch, out semanticVersion);
        Major = major;
        Minor = minor;
        Patch = patch;

        if (semanticVersion != null)
        {
            if (semanticVersion.Equals("test", StringComparison.OrdinalIgnoreCase))
            {
                IsTest = true;
            }
            else
            {
                throw new ArgumentException("semVer");
            }
        }
    }

    private TaskVersion(TaskVersion taskVersionToClone)
    {
        this.IsTest = taskVersionToClone.IsTest;
        this.Major = taskVersionToClone.Major;
        this.Minor = taskVersionToClone.Minor;
        this.Patch = taskVersionToClone.Patch;
    }

    public TaskVersion(int major, int minor, int overidePatch)
    {
        if (overidePatch < 0)
        {
            throw new Exception($"Bug, overridePatch must be >=0, got {overidePatch}");
        }

        Major = major;
        Minor = minor;
        Patch = overidePatch;
    }

    public Int32 Major
    {
        get;
        set;
    }

    public Int32 Minor
    {
        get;
        set;
    }

    public Int32 Patch
    {
        get;
        set;
    }

    public Boolean IsTest
    {
        get;
        set;
    }

    public TaskVersion Clone()
    {
        return new TaskVersion(this);
    }

    public TaskVersion CloneWithMinorAndPatch(int minor, int overridePatch)
    {
        return new TaskVersion(Major, minor, overridePatch);
    }

    public TaskVersion CloneWithPatch(int overridePatch)
    {
        return new TaskVersion(Major, Minor, overridePatch);
    }

    public TaskVersion CloneWithMajor(int major)
    {
        return new TaskVersion(major, Minor, Patch);
    }

    public static implicit operator String(TaskVersion version)
    {
        return version.ToString();
    }

    public override String ToString()
    {
        String suffix = String.Empty;
        if (IsTest)
        {
            suffix = "-test";
        }

        return String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}{3}", Major, Minor, Patch, suffix);
    }


    internal string MinorPatchToString()
    {
        String suffix = String.Empty;
        if (IsTest)
        {
            throw new NotImplementedException();
        }

        return String.Format(CultureInfo.InvariantCulture, "{1}.{2}{3}", Major, Minor, Patch, suffix);
    }

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    public Int32 CompareTo(TaskVersion? other)
    {
        if (other is null)
        {
            throw new ArgumentNullException("other");
        }

        Int32 rc = Major.CompareTo(other.Major);
        if (rc == 0)
        {
            rc = Minor.CompareTo(other.Minor);
            if (rc == 0)
            {
                rc = Patch.CompareTo(other.Patch);
                if (rc == 0 && this.IsTest != other.IsTest)
                {
                    rc = this.IsTest ? -1 : 1;
                }
            }
        }

        return rc;
    }

    public Boolean Equals(TaskVersion? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.CompareTo(other) == 0;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TaskVersion);
    }

    public static Boolean operator ==(TaskVersion v1, TaskVersion v2)
    {
        if (v1 is null)
        {
            return v2 is null;
        }

        return v1.Equals(v2);
    }

    public static Boolean operator !=(TaskVersion v1, TaskVersion v2)
    {
        if (v1 is null)
        {
            return !(v2 is null);
        }

        return !v1.Equals(v2);
    }

    public static Boolean operator <(TaskVersion v1, TaskVersion v2)
    {
        ArgumentUtility.CheckForNull(v1, nameof(v1));
        ArgumentUtility.CheckForNull(v2, nameof(v2));
        return v1.CompareTo(v2) < 0;
    }

    public static Boolean operator >(TaskVersion v1, TaskVersion v2)
    {
        ArgumentUtility.CheckForNull(v1, nameof(v1));
        ArgumentUtility.CheckForNull(v2, nameof(v2));
        return v1.CompareTo(v2) > 0;
    }

    public static Boolean operator <=(TaskVersion v1, TaskVersion v2)
    {
        ArgumentUtility.CheckForNull(v1, nameof(v1));
        ArgumentUtility.CheckForNull(v2, nameof(v2));
        return v1.CompareTo(v2) <= 0;
    }

    public static Boolean operator >=(TaskVersion v1, TaskVersion v2)
    {
        ArgumentUtility.CheckForNull(v1, nameof(v1));
        ArgumentUtility.CheckForNull(v2, nameof(v2));
        return v1.CompareTo(v2) >= 0;
    }
}

