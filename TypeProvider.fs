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

and Algorithm =
    | Greedy

and SettingsBase =
    {
        (* TODO: Implement settings
        Data: string option
        Timeslots: int option
        *)
        Algorithm: string option
        Room: int option
    }

and Settings =
    {
        (* TODO: Implement settings
        Data: string option
        Timeslots: int option
        *)
        Algorithm: Algorithm
        Room: int option
    }