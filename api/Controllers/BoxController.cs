using api.Filter;
using api.Helper;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;


public class BoxController : ControllerBase
{
    private readonly ILogger<BoxController> _logger;
    private readonly BoxService _boxService;
    private readonly ResponseHelper _responseHelper;

    public BoxController (ILogger<BoxController> logger,BoxService boxService,ResponseHelper responseHelper)
    {
        _logger = logger;
        _boxService = boxService;
        _responseHelper = responseHelper;
    }

    [HttpGet]
    [Route("/factory/catalog")]

    public ResponseDto GetAllBoxes()
    {
        return _responseHelper.Success(HttpContext, 200, "Boxes catalog fetched successfully",
            _boxService.GetAllBoxes());
    }

    [HttpPost]
    [ValidationModel]
    [Route("/catalog/boxes")]

    public ResponseDto CreateBox([FromBody] CreateNewBoxRequest dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully create a new box",
            ResponseData = _boxService.CreateBox(dto.BoxName, dto.Description, dto.Size, dto.Price, dto.ImageUrl)
        };
    }

    [HttpPut]
    [ValidationModel]
    [Route("/catalog/boxes/{boxId}")]

    public ResponseDto UpdateBoxById([FromRoute] int boxId,
        [FromBody] UpdateBoxRequest dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _boxService.UpdateBox(dto.BoxId, dto.BoxName, dto.Description, dto.Size, dto.Price,
                dto.ImageUrl)
        };
    }

    [HttpGet]
    [Route("/catalog/boxes/{boxId}")]

    public Box GetBoxById([FromRoute] int boxId)
    {
        var box = _boxService.GetBoxById(boxId);
        return box;
    }

    [HttpDelete]
    [Route("catalog/boxes/{boxId}")]

    public ResponseDto DeleteBox([FromRoute] int boxId)
    {
        _boxService.DeleteBox(boxId);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted "
        };
    }

    [HttpGet]
    [ValidationModel]
    [Route("/catalog/boxes")]

    public IEnumerable<BoxFeedQuery> SearchBox([FromQuery] SearchBoxDto searchBoxDto)
    {
        return _boxService.SearchBox(searchBoxDto.SearchTerm, searchBoxDto.PageSize);
    }
}