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

and Settings =
    {
        (* TODO: Implement settings
        Algorithm: string option
        Data: string option
        Timeslots: int option
        *)
        Room: int option
    }