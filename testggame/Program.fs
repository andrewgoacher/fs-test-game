namespace testggame
module Program=
    open System

    [<EntryPoint>]
    [<STAThread>]
    let main _ =
        use game = new Game.Game()
        game.Run()
        0