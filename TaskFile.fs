namespace FILE

module File =

    open System
    open System.IO

    let getFileName () = "tasks.log"

    let getAllTasks () =
        let filename = getFileName()
        let content = File.ReadAllText filename
        let allTasks = content.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries)
        allTasks

    let saveFile (tasks: string list) =
        let filename = getFileName()
        File.WriteAllText(filename, String.Empty)
        for task in tasks do
            File.AppendAllText(filename, $"{task.Trim()};\n")
