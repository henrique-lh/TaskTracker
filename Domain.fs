module Domain

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

let parseStatus (statusStr: string) =
    match statusStr.Trim().ToLower() with
    | "todo" -> Todo
    | "doing" -> Doing
    | "done" -> Done
    | _ -> failwith "Status desconhecido"
