pragma solidity >=0.4.22;

contract Deposit {

    address admin; // contract onwer
    string privateKey;
    
        struct Money{
        address addr;
        uint amount;
    }
        Money[] public moneys;
    
    constructor(string memory _privateKey) public payable{
        
        privateKey = _privateKey;
        admin=msg.sender;
    }

    function() external payable{}
    
    function deposit() public payable {}
    
    function withdraw(string memory _privateKey, uint _balance) public payable{
      
        require(keccak256(abi.encodePacked(_privateKey)) == keccak256(abi.encodePacked(privateKey)));
        msg.sender.transfer(_balance);
        
        Money memory withdrawAmount = Money(msg.sender, _balance);
        moneys.push(withdrawAmount);
        
    }
    
    //get all 
    function withdrawAll() public  {
        
        require(msg.sender == admin);
      
        msg.sender.transfer(address(this).balance);
    }
    
    function getBalance() public view  returns (uint){
        
        return address(this).balance;
    }

    function getKey() public view  returns (string memory){
        
        require(msg.sender == admin);
        
        return privateKey;
    }
    
    function getMoneyLength() public view  returns (uint){
        
        return moneys.length;
    }

 
}
