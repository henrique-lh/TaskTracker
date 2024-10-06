namespace CRUDE

module CRUDE =
    open System
    open System.IO
    open Domain
    open FILE.File

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
