module GraphColouring.TypeProvider

[<AutoOpen>]
type Student =
    {
        Id: string
        Courses: string list
        CoursesQuantity: int
    }

and Exam =
    {
        Id: string
        Students: string list
        StudentsQuantity: int
    }

and Timetable =
    {
        Timeslots: Map<int, string list>
    }

and GraphType =
    | Constraint
    | Greedy
    | Random

and Algorithm =
    | All
    | Greedy
    | Random

and SettingsBase =
    {
        (* TODO: Implement settings
        Data: string option
        Timeslots: int option // i.e. number of columns in output table
        *)
        Algorithm: string option
        Room: int option // i.e. number of rows in output table
    }

and Settings =
    {
        (* TODO: Implement settings
        Data: string option
        Timeslots: int option // i.e. numberof columns in output table
        *)
        Algorithm: Algorithm
        Room: int option // i.e. number of rows in output table
    }