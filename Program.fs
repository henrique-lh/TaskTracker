open System
open System.IO
open System.Collections.Generic
open FSharp.CommandLine

type Status =
| Todo
| Doing
| Done
| All

type Task = {
    id: int;
    name: string;
    status: Status;
}

let queue = new Queue<string>()
let log (logValue: string) = queue.Enqueue logValue

let parseStatus (statusStr: string) =
    match statusStr.Trim().ToLower() with
    | "todo" -> Todo
    | "doing" -> Doing
    | "done" -> Done
    | _ -> failwith "Status desconhecido"

let getFileName () = "tasks.log"

let getAllTasks () =
    let filename = getFileName()
    let content = File.ReadAllText filename
    let allTasks = content.Split([| ';' |], StringSplitOptions.RemoveEmptyEntries)
    allTasks

let listTasks () =
    
    let tasks = getAllTasks()
    if tasks.Length = 0 then
        printfn "You do not have any task!"

    for task in tasks do
        let fields = task.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
        if fields.Length = 3 then
            // Parse dos campos da tarefa
            let id = fields.[0].Trim() |> int
            let name = fields.[1].Trim()
            let status = parseStatus fields.[2]
            
            // Criar a tarefa e imprimir
            let task = { id = id; name = name; status = status }
            printfn "ID: %d | Nome: %s | Status: %A" task.id task.name task.status
        else
            printfn "Formato inválido para a tarefa: %s" task

let listTasksCommand =
    commandOption {
        names ["list"]
        description "List all tasks"
        defaultValue ""
    }

let addTask (task: string) =
    let filename = getFileName()
    let tasks = getAllTasks()
    let countTasks = tasks.Length
    let id =
        if countTasks = 0 then
            1
        else
            let candidateId = countTasks + 1
            let maxId =
                tasks 
                |> Array.map (fun v ->
                    let fields = v.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
                    let _id = fields.[0].Trim() |> int
                    _id
                )
                |> Array.toList
                |> List.max
            Math.Max (candidateId, maxId + 1)
            
    let task = { id = id; name = task; status = Todo }
    let taskString =
        if countTasks = 0 then
            $"{task.id},{task.name},{task.status};"
        else
            $"\n{task.id},{task.name},{task.status};"
    File.AppendAllText(filename, taskString)
    printfn "Task added succesfully (ID: %d)" id

let addTaskCommand =
    commandOption {
        names ["add"]
        description "Add a task to the tasks.txt"
        takes(format("%s").map (fun item -> item))
    }

let updateTask (id: int, taskName: string) =
    let tasks = getAllTasks()
    let countTasks = tasks.Length
    if id > countTasks || id < 1 then
        printfn "Task could not be updated. Task does not exist"
    let filename = getFileName()
    let task = { id = id; name = taskName; status = Todo }
    let taskString =
        if id = 1 then
            $"{task.id},{task.name},{task.status}"
        else
            $"\n{task.id},{task.name},{task.status}"
    let updatedTasks = 
        tasks |> Array.map (fun v ->
            let fields = v.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
            let _id = fields.[0].Trim() |> int
            if id = _id then taskString.Trim()
            else v.Trim()
        )
    File.WriteAllText(filename, String.Empty)
    let mutable c = 0
    for updatedTask in updatedTasks do
        let updatedTaskString = 
            if c = 0 then
                $"{updatedTask};"
            else
                $"\n{updatedTask};"
        c <- c + 1
        File.AppendAllText(filename, updatedTaskString)

let updateTaskCommand =
    commandOption {
        names ["update"]
        description "Update a task"
        takes(format("%i %s").withNames ["idTask"; "newTask"])
    }

let deleteTask (id: int) =
    let tasks = getAllTasks()
    let countTasks = tasks.Length
    if id > countTasks || id < 1 then
        printfn "Task could not be deleted. Task does not exist"
    let filename = getFileName()
    let updatedTasks = 
        tasks |> Array.filter (fun v ->
            let fields = v.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
            let _id = fields.[0].Trim() |> int
            _id <> id
        )
    File.WriteAllText(filename, String.Empty)
    let mutable c = 0
    for updatedTask in updatedTasks do
        let updatedTaskString = updatedTask.Trim()
        let updatedTaskString = 
            if c = 0 then
                $"{updatedTaskString};"
            else
                $"\n{updatedTaskString};"
        c <- c + 1
        File.AppendAllText(filename, updatedTaskString)

let deleteTaskCommand =
    commandOption {
        names ["delete"]
        description "Delete a task"
        takes(format("%i").withNames ["idTask"])
    }

let updateStatusTask (status: string, id: int) =
    let parsedStatus = parseStatus status
    let filename = getFileName()
    let tasks = getAllTasks()
    let updatedTasks =
        tasks |> Array.mapi(fun i v ->
            let fields = v.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
            let _id = fields.[0].Trim() |> int
            let name = fields.[1].Trim()
            if _id = id then
                let t = { id = id; name = name; status = parsedStatus }
                $"{t.id},{t.name},{t.status}"
            else
                v.Trim()
        )
    File.WriteAllText(filename, String.Empty)
    let mutable c = 0
    for updatedTask in updatedTasks do
        let updatedTaskString = 
            if c = 0 then
                $"{updatedTask};"
            else
                $"\n{updatedTask};"
        c <- c + 1
        File.AppendAllText(filename, updatedTaskString)

let updateStatusTaskCommand =
    commandOption {
        names ["status"]
        description "Update status of a task"
        takes(format("%s %i").withNames ["status"; "idTask"])
    }

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
