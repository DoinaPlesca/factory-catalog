using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repository;

namespace service;

public class BoxService
{
    private readonly BoxRepository _boxRepository;

    public BoxService(BoxRepository boxRepository)
    {
        _boxRepository = boxRepository;
    }

    public IEnumerable<BoxFeedQuery> GetAllBoxes()
    {
        return _boxRepository.GetAllBoxes();
    }

    public Box CreateBox(string boxName, string description, string size, int price, string imageUrl)
    {
        return _boxRepository.CreateBox(boxName, description, size, price, imageUrl);
    }

    public Box GetBoxById(int boxId)
    {
        return _boxRepository.GetBoxById(boxId);
    }

    public Box UpdateBox(int boxId, string boxName, string description, string size, int price, string imageUrl)
    {
        return _boxRepository.UpdateBox(boxName, description, size, price, imageUrl);
    }

    public bool DeleteBox(int boxId)
    {
        return _boxRepository.DeleteBox(boxId);
    }

    public IEnumerable<BoxFeedQuery> SearchBox(string searchDtoSearchTerm, int searchDtoPageSize)
    {
        return _boxRepository.SearchBoxes(searchDtoSearchTerm, searchDtoPageSize);
    }
}

