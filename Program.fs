open FSharp.CommandLine
open CLI
open CRUDE.CRUDE

let mainCommand () =
    command {
        opt fileToRead in listTasksCommand |> CommandOption.zeroOrExactlyOne
        opt newTask in addTaskCommand |> CommandOption.zeroOrExactlyOne
        opt updatedTask in updateTaskCommand |> CommandOption.zeroOrExactlyOne
        opt deletedTask in deleteTaskCommand |> CommandOption.zeroOrExactlyOne
        opt updatedStatusTask in updateStatusTaskCommand |> CommandOption.zeroOrExactlyOne
        do
            match fileToRead with
            | Some _ -> listTasks()
            | _ -> ()

        do
            match newTask with
            | Some task -> addTask task
            | _ -> ()

        do
            match updatedTask with
            | Some (id, task) -> updateTask(id, task)
            | _ -> ()

        do
            match deletedTask with
            | Some id -> deleteTask id
            | _ -> ()

        do
            match updatedStatusTask with
            | Some (status, id) -> updateStatusTask(status, id)
            | _ -> ()

        return 0
    }

[<EntryPoint>]
let main argv =
    mainCommand() |> Command.runAsEntryPoint argv
