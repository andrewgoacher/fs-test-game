module Animation

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Texture

type Frame = {atlasItem:AtlasSprite;frameDuration:float}
type Animation = {name:string;frames:Frame list;loops:bool}
type Animations = {animations:Animation list;currentAnimation:string}
type AnimationState = {
    animations:Animations
    position:Vector2
    frameTime:float
    active:bool
    currentFrame:int
}

let createFrame sprite duration =
    {atlasItem=sprite;frameDuration=duration}

let createAnimation name frames loops =
    {name=name;frames=frames;loops=loops}

let createAnimationFromAtlas name atlas indices duration loops =
    let frames = 
        indices
        |> List.map (fun index ->
            let sprite =Texture.createAtlasSprite atlas index atlas.items.[index].source
            createFrame sprite duration
        )
    createAnimation name frames loops 

let addAnimation (animations:Animations) (animation :Animation)=
    let currentAnimations = animations.animations
    match currentAnimations with
    | [] -> {animations=[animation];currentAnimation=animation.name}
    | list -> {animations=animation::list;currentAnimation=animations.currentAnimation}

let createAnimationState animations position frameTime isActive currentFrame =
    {animations=animations;position=position;frameTime=frameTime;active=isActive;currentFrame=currentFrame}

let isActive animationState =
    animationState.active

let position animationState =
    animationState.position

let currentAnimation animation =
    let animationCollection = animation.animations
    let name = animationCollection.currentAnimation
    let selectedAnimation = 
        animationCollection.animations 
        |> List.tryPick (fun anim ->
           if anim.name.Equals(name) then
               Some anim
           else None)

    match selectedAnimation with
    | Some a -> a
    | None -> failwith "not found"

let frame animationState animation =
    let frames = animation.frames
    let frameRef = animationState.currentFrame
    frames.[frameRef]

let draw (spritebatch:SpriteBatch) (currentState:AnimationState) =
   if not <| isActive currentState then
    ()
   else
        let position = position currentState
        let currentAnimation = currentAnimation currentState
        let frame = frame currentState currentAnimation
        let source = Texture.source <| Texture.AtlasSprite frame.atlasItem
        let bounds = Math.createRect (int position.X) (int position.Y) source.Width source.Height

        Texture.draw spritebatch (Texture.AtlasSprite frame.atlasItem) (Some bounds)

let update (gameTime:GameTime) (currentState:AnimationState) =
    if not <| isActive currentState then
        currentState
    else
        let elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds
        let newFrameTime = currentState.frameTime + elapsedTime
        let currentAnimation = currentAnimation currentState
        let currentFrame = frame currentState currentAnimation
        let duration = currentFrame.frameDuration

        let incrementFrame animationState animation =
            let currentFrame = animationState.currentFrame
            let frameCount = List.length animation.frames

            if currentFrame < frameCount-1 then
                (currentFrame + 1, true)
            else
                if animation.loops then
                    (0,true)
                else (currentFrame,false)

        if newFrameTime >= duration then
            let frameRef,isactive = incrementFrame currentState currentAnimation
            createAnimationState currentState.animations currentState.position 0.0 isactive frameRef
        else
            createAnimationState currentState.animations currentState.position newFrameTime currentState.active currentState.currentFrame