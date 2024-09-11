## Neuro-sama Integration Mod for Who Wants To Be A Millionaire?

This mod for [Who Wants To Be A Millionaire?](https://store.steampowered.com/app/1356240) opens a websocket to allow programmatic control of the game.

Apart from that, there are various patches made to make the game more stream-friendly, such as removing repeated voice lines and allowing external control over the Ask the Audience and Phone a Friend lifelines.

The menus are navigated automatically, but you can hold Shift to take manual control. Additionally, you can press Q to resend the question over the websocket.

Please note that there is a bug which makes it so that if you press Space, even if the game is unfocused, it will skip certain actions which might cause problems.

## Usage

This mod needs BepInEx 6 mod loader.

If you use this mod for your own content creation purposes, it would be cool if you credited [AlexejheroDev](https://twitch.tv/AlexejheroDev) and [vedal987](https://twitch.tv/vedal987).

## Websocket Schema

There is a sample websocket server in the WSTest folder to help.

All Websocket messages (both C2S and S2C) have the following format:

```ts
{
    Command: string,
    Data?: {[key: string]: any}
}
```

## Mod to server (C2S)

### Client connected
```ts
{
    Command: "client"
}
```
Sent when the client is initialized. You can ignore this server-side.

### Game launched
```ts
{
    Command: "launched"
}
```
Sent when the client is launched. 

You should respond to this with [Select language](#select-language).

### Ready to start
```ts
{
    Command: "ready",
    Data: {
        availableDifficulties: [ "easy", "normal" ],
        availableCategories: string[]
    }
}
```
Sent when the game is ready to start. 

You should respond to this with [Start game](#start-game).

### Inspect character
```ts
{
    Command: "character",
    Data: {
        name: string,
        age: string,
        profession: string,
        passion: string
    }
}
```
Sent when a character is scrolled to in-game during the character select screen. 

You should respond to this event with [Character response](#character-response).

### Question
```ts
{
    Command: "question"
    Data: {
        question: string,
        answerA?: string,
        answerB?: string,
        answerC?: string,
        answerD?: string,
        lifelines: ("50_50" | "phone_a_friend" | "ask_the_audience" | "flip_the_question")[]
    }
}
```
Sent when the game prompts a new question. The lifelines array will contain the remaining available lifelines. Note that after a 50/50 lifeline is used, two of the answer properties will be missing (undefined).

You should respond to this event with [Answer](#answer) or [Use lifeline](#use-lifeline).

### Answer correct
```ts
{
    Command: "answer/correct"
}
```
Sent when a question is answered correctly.

### Confirm lifeline Ask the Audience
```ts
{
    Command: "lifeline/ask_the_audience/confirm_start"
}
```
Sent when the Ask the Audience lifeline is started in the game. You should use this event to run your own Ask the Audience logic.

You should respond to this event with [Ask the Audience results](#ask-the-audience-results) once your custom logic is finished.

### Confirm lifeline Phone a Friend
```ts
{
    Command: "lifeline/phone_a_friend/confirm_start"
}
```
Sent when the Phone a Friend lifeline is started in the game. You should use this event to run your own Phone a Friend logic.

### Start Phone a Friend ringtone
```ts
{
    Command: "lifeline/phone_a_friend/ringtone_start"
}
```
Sent after the Phone a Friend voice lines and camera animations are finished in the game. You can use this event to play a ringtone sound to simulate a call.

You can respond to this event with [Phone a Friend results](#phone-a-friend-results) once your custom logic is finished, if it is finished early.

### End Phone a Friend
```ts
{
    Command: "lifeline/phone_a_friend/request_end"
}
```
Sent after the 30s are up and the Phone a Friend lifeline has ended. You should use this event to end your Phone a Friend logic.

You should respond to this event with [Phone a Friend results](#phone-a-friend-results) as soon as possible.

### Finish game
```ts
{
    Command: "finish",
    Data: {
        won: boolean
    }
}
```
Sent when the game ends, which happens if a question is answered incorrectly or if all 15 questions were answered correctly.

## Server to mod (S2C)

### Select language
```ts
{
    Command: "language",
    Data: {
        language: "EN_UK" | "EN_US" | "FR" | "ALL" | "ITA" | "ESP"
    }
}
```
Send this to choose the language in which you want to play the game. `ALL` is for German.

### Start game
```ts
{
    Command: "start",
    Data: {
        difficulty: "easy" | "normal",
        categories: string[]
    }
}
```
Send this to start the game. If you choose the `easy` difficulty, it does not matter what categories you choose. The `categories` array should be identical to or a subset of the `availableCategories` array sent in [Ready to start](#ready-to-start)

### Character response
```ts
{
    Command: "character/response",
    Data: {
        accept: boolean
    }
}
```
Send this to either accept the chosen character and start the game or to move on to the next character.

### Answer
```ts
{
    Command: "answer",
    Data: {
        answer: "A" | "B" | "C" | "D"
    }
}
```
Send this to answer a question. Please ensure that you don't send an answer that has already been removed by the 50/50 lifeline.

### Use lifeline
```ts
{
    Command: "lifeline",
    Data: {
        lifeline: "50_50" | "phone_a_friend" | "ask_the_audience" | "flip_the_question"
    }
}
```
Send this to attempt to use a lifeline. Do not run your own lifeline logic until the mod sends [Confirm lifeline Ask the Audience](#confirm-lifeline-ask-the-audience) or [Confirm lifeline Phone a Friend](#confirm-lifeline-phone-a-friend).

After `50_50` or `flip_the_question`, the mod will send back the new [Question](#question).

If you try to use a lifeline that has already been used, nothing will happen.

### Ask the Audience results
```ts
{
    Command: "lifeline/ask_the_audience/results",
    Data: {
        percentageA: number,
        percentageB: number,
        percentageC: number,
        percentageD: number
    }
}
```
Send this after your custom Ask the Audience logic is finished. The percentage fields are integers from `0` to `100`.

Keep in mind that the game's regular Ask the Audience music has been extended to just above 1 minute, so your poll should take 1 minute or less.

If this lifeline is called after 50/50 and the current question has two missing answers, it is your job to prevent those answers from being added to the poll, and your job to send those percentages back as `0`.

### Phone a Friend results
```ts
{
    Command: "lifeline/phone_a_friend/results",
    Data: {
        result: "A" | "B" | "C" | "D" | "?"
    }
}
```
Send this after your custom Phone a Friend logic is finished. If your logic is interrupted by [End Phone a Friend](#end-phone-a-friend), you still need to send this as soon as possible.

The `result` will be displayed on-screen, however if you cannot determine what was suggested by the friend or you simply don't want to bother doing it, you can send back `?`.
