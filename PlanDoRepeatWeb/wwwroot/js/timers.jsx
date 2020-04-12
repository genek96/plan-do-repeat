function Play(props){
    return (<div className="timer-button timer-button-play" onClick={props.onClick}/>);    
}

function Pause(props){
    return (<div className="timer-button timer-button-pause" onClick={props.onClick}/>);
}

class Timer extends React.Component {
    constructor(props){
        super(props);
        this.state = {
            name: props.initial.Name,
            descripition: props.initial.Description,
            lastUpdate: props.initial.LastUpdate,
            elapsedTime: props.initial.PassedSeconds,
            status: props.initial.State  
        };
    }

    tick(){
        if (this.state.status !== 1){
            return;
        }
        this.setState({
            name: this.state.name,
            descripition: this.state.descripition,
            lastUpdate: this.state.lastUpdate,
            elapsedTime: this.state.elapsedTime + 1,
            status: this.state.status
        });
    }
    
    componentDidMount() {
        this.interval = setInterval(() => this.tick(), 1000);
    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }
    
    onClickPlay(){
        if (this.state.status === 1){
            return;
        }

        const isSuccess = fetch("/TimerApi/StartTimer?timerId=" + this.props.id,{method: 'POST'})
            .then(res => status < 300);
        
        if (isSuccess){
            this.setState({
                name: this.state.name,
                descripition: this.state.descripition,
                lastUpdate: Date.now(),
                elapsedTime: this.state.elapsedTime,
                status: 1
            });
        }
    }
    
    onClickRepeat(){
        this.setState({
            name: this.state.name,
            descripition: this.state.descripition,
            lastUpdate: Date.now(),
            elapsedTime: 0,
            status: 1
        });
    }

    onClickStop(){
        if (this.state.status === 0){
            return;
        }

        const isSuccess = fetch("/TimerApi/StopTimer?timerId=" + this.props.id,{method: 'POST'})
            .then(res => status < 300);

        if (isSuccess){
            this.setState({
                name: this.state.name,
                descripition: this.state.descripition,
                lastUpdate: Date.now(),
                elapsedTime: 0,
                status: 0
            });
        }
    }

    onClickPause(){
        if (this.state.status === 2){
            return;
        }

        const isSuccess = fetch("/TimerApi/PauseTimer?timerId=" + this.props.id,{method: 'POST'})
            .then(res => status < 300);

        if (isSuccess){
            this.setState({
                name: this.state.name,
                descripition: this.state.descripition,
                lastUpdate: Date.now(),
                elapsedTime: this.state.elapsedTime > 0 ? this.state.elapsedTime : 0,
                status: 2
            });
        }
    }
    
    onClickDelete(){
        if (!confirm("Вы уверены, что хотите удалить таймер: \"" + this.state.name + "\"")){
            return;
        }
        const isSuccess = fetch("/TimerApi/DeleteTimer?timerId=" + this.props.id,{method: 'POST'})
            .then(res => status < 300);
        if (isSuccess){
            //todo refresh list
        }
    }
    
    render() {
        const elapsedSeconds = this.props.initial.PeriodInSeconds - this.state.elapsedTime;
        return (<div className="timer">
            <div className="timer-body">{this.state.name} : {this.state.descripition} : {elapsedSeconds}</div>
            <div className="timer-button timer-button-repeat" onClick={() => this.onClickRepeat()}/>
            {this.state.status !== 1
                ? <Play onClick={() => this.onClickPlay()} />
                : <Pause onClick={() => this.onClickPause()} />}
            <div className="timer-button timer-button-stop" onClick={() => this.onClickStop()}/>
            <div className="timer-button timer-button-delete" onClick={() => this.onClickDelete()}/>
        </div>);
    }
}

class TimerList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            error: null,
            isLoaded: false,
            timers: []
        };
    }
    
    renderTimer(timerProps){
        return (<Timer key={timerProps.Id}
                       id={timerProps.Id}
                       initial={timerProps}
        />);
    }

    componentDidMount() {
        fetch("/TimerApi/GetAllTimers")
            .then(res => res.json())
            .then(
                result => {
                    this.setState({
                        error: null,
                        isLoaded: true,
                        timers: result
                    });
                },
                (error) => {
                    this.setState({
                        error: error.message,
                        isLoaded: true,
                        timers: null
                    });
                })
    }

    render() {
        let status = "";
        if (!this.state.isLoaded) {
            status = "Загрузка...";
        } else if (this.state.error) {
            status = this.state.error;
        } else {
            status = this.state.timers.map(item => this.renderTimer(item));
        }
        return (<div className="timers-list">{status}</div>);
    }
}

ReactDOM.render(
    <TimerList/>,
    document.getElementById("content")
);