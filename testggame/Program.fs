namespace testggame
module Program=
    open System

    [<EntryPoint>]
    [<STAThread>]
    let main argv =
        use game = new Game.Game()
        game.Run()
        0