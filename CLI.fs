module CLI

open System.Collections.Generic
open FSharp.CommandLine

let queue = new Queue<string>()
let log (logValue: string) = queue.Enqueue logValue

let listTasksCommand =
    commandOption {
        names ["list"]
        description "List all tasks"
        defaultValue ""
    }

let addTaskCommand =
    commandOption {
        names ["add"]
        description "Add a task to the tasks.txt"
        takes(format("%s").map (fun item -> item))
    }

let updateTaskCommand =
    commandOption {
        names ["update"]
        description "Update a task"
        takes(format("%i %s").withNames ["idTask"; "newTask"])
    }

let deleteTaskCommand =
    commandOption {
        names ["delete"]
        description "Delete a task"
        takes(format("%i").withNames ["idTask"])
    }

let updateStatusTaskCommand =
    commandOption {
        names ["status"]
        description "Update status of a task"
        takes(format("%s %i").withNames ["status"; "idTask"])
    }
